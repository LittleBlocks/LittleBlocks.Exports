using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Easify.Exports.Storage;
using Easify.Exports.Extensions;
using Microsoft.Extensions.Logging;
using NetBox.Extensions;
using Polly;

namespace Easify.Exports.Csv
{
    public class CsvFileWriter : ICsvFileWriter
    {
        private readonly ICsvStorageTargetResolver _storageTargetResolver;
        private readonly ILogger<CsvFileWriter> _logger;

        public CsvFileWriter(ICsvStorageTargetResolver storageTargetResolver, ILogger<CsvFileWriter> logger)
        {
            _storageTargetResolver = storageTargetResolver ?? throw new ArgumentNullException(nameof(storageTargetResolver));
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

            var tasks = configuration.Targets.Select(async t=>
            {
                var targetType = _storageTargetResolver.Resolve(t.StorageTargetType);
                if (targetType == null)
                {
                    var message = $"The ExportStorage for {t.StorageTargetType} is not found. Make sure it has been registered in container correctly.";
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

                throw new StorageTargetException(new []{ex});
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
                        TimeSpan.FromSeconds(target.RetryDelay*2),
                        TimeSpan.FromSeconds(target.RetryDelay*4)
                    },
                    (exception, timeSpan) =>
                    {
                        _logger.LogError($"Error in writing file to target. Retry again in {timeSpan.Seconds}s", exception);
                    });

            await retry.ExecuteAsync(async () =>
            {
                await targetType.WriteAsync(target.TargetLocation, configuration.FileName, byteArray);
            });
        }

        private async Task<byte[]> GenerateFileAsync<T>(IEnumerable<T> items, CsvExportConfiguration configuration) where T : class
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
