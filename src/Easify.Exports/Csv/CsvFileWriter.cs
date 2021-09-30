// This software is part of the Easify.Exports Library
// Copyright (C) 2021 Intermediate Capital Group
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Easify.Exports.Extensions;
using Easify.Exports.Storage;
using Microsoft.Extensions.Logging;
using NetBox.Extensions;
using Polly;

namespace Easify.Exports.Csv
{
    public class CsvFileWriter : ICsvFileWriter
    {
        private readonly ILogger<CsvFileWriter> _logger;
        private readonly ICsvStorageTargetResolver _storageTargetResolver;

        public CsvFileWriter(ICsvStorageTargetResolver storageTargetResolver, ILogger<CsvFileWriter> logger)
        {
            _storageTargetResolver =
                storageTargetResolver ?? throw new ArgumentNullException(nameof(storageTargetResolver));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task WriteFileAsync<T>(IEnumerable<T> items, CsvExportConfiguration configuration) where T : class
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var enumerable = items as T[] ?? items.ToArray();

            _logger.LogInformation($"Writing {enumerable.Length} {typeof(T)} to {configuration.Targets.ToJson()}");

            var byteArray = await GenerateFileAsync(enumerable, configuration);

            _logger.LogInformation($"Writing contents for {typeof(T)} to the target storage list");

            var tasks = configuration.Targets.Select(async t =>
            {
                var targetType = _storageTargetResolver.Resolve(t.StorageTargetType);
                if (targetType == null)
                {
                    var message =
                        $"The ExportStorage for {t.StorageTargetType} is not found. Make sure it has been registered in container correctly.";
                    _logger.LogError(message);

                    throw new StorageTargetNotFoundException(message);
                }

                await WriteToTargetTypeAsync(configuration, targetType, t, byteArray);
            }).ToArray();

            try
            {
                _logger.LogInformation($"Wait for all storage exports for {typeof(T)} to complete");

                await Task.WhenAll(tasks);

                _logger.LogInformation($"All the exports for {typeof(T)} are completed");
            }
            catch (AggregateException ex)
            {
                var exception = new StorageTargetException(ex.InnerExceptions);
                _logger.LogError($"Error in exporting to one or more storage. {exception.Message}", ex);

                throw exception;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in exporting to one or more storage. {ex.Message}", ex);

                throw new StorageTargetException(new[] {ex});
            }
        }

        private async Task WriteToTargetTypeAsync(CsvExportConfiguration configuration, ICsvStorageTarget targetType,
            StorageTarget target, byte[] byteArray)
        {
            var retry = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(new[]
                    {
                        TimeSpan.FromSeconds(target.RetryDelay),
                        TimeSpan.FromSeconds(target.RetryDelay * 2),
                        TimeSpan.FromSeconds(target.RetryDelay * 4)
                    },
                    (exception, timeSpan) =>
                    {
                        _logger.LogError($"Error in writing file to target. Retry again in {timeSpan.Seconds}s",
                            exception);
                    });

            await retry.ExecuteAsync(async () =>
            {
                await targetType.WriteAsync(target.TargetLocation, configuration.FileName, byteArray);
            });
        }

        private async Task<byte[]> GenerateFileAsync<T>(IEnumerable<T> items, CsvExportConfiguration configuration)
            where T : class
        {
            _logger.LogInformation($"Generating file content for {typeof(T)} output");

            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var csvWriter = new CsvWriter(writer, configuration.Configuration))
            {
                csvWriter.Context.SetupFromCsvConfiguration(configuration);

                await csvWriter.WriteRecordsAsync(items);
                await csvWriter.FlushAsync();
                await writer.FlushAsync();
                await stream.FlushAsync();

                stream.Position = 0;
                return stream.ToByteArray();
            }
        }
    }
}