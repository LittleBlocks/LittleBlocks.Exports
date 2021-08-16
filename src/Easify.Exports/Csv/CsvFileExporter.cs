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
using System.Linq;
using System.Threading.Tasks;
using Easify.Exports.Extensions;
using Microsoft.Extensions.Logging;

namespace Easify.Exports.Csv
{
    public sealed class CsvFileExporter : IFileExporter
    {
        private readonly ICsvExportConfigurationBuilder _csvExportConfigurationBuilder;
        private readonly ICsvFileWriter _csvFileWriter;
        private readonly ILogger<CsvFileExporter> _logger;

        public CsvFileExporter(ICsvFileWriter csvFileWriter,
            ICsvExportConfigurationBuilder csvExportConfigurationBuilder,
            ILogger<CsvFileExporter> logger)
        {
            _csvFileWriter = csvFileWriter ?? throw new ArgumentNullException(nameof(csvFileWriter));
            _csvExportConfigurationBuilder = csvExportConfigurationBuilder ??
                                             throw new ArgumentNullException(nameof(csvExportConfigurationBuilder));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ExportResult> ExportAsync<T>(IEnumerable<T> items, ExporterOptions options) where T : class
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (options == null) throw new ArgumentNullException(nameof(options));

            var entities = items as T[] ?? items.ToArray();
            if (!entities.Any())
            {
                var error = $"Export for {typeof(T)} has cancelled due to an empty list of items";
                _logger.LogWarning(error);

                return ExportResult.Fail(error);
            }

            try
            {
                var configuration = _csvExportConfigurationBuilder.Build<T>(options);
                if (configuration == null)
                {
                    var error =
                        $"Error in creating export configuration '{typeof(T)}'. Make sure the ClassMap has been registered";
                    _logger.LogWarning(error);

                    return ExportResult.Fail(error);
                }

                await _csvFileWriter.WriteFileAsync(entities, configuration);

                _logger.LogInformation($"File {configuration.FileName} written for '{typeof(T)}' " +
                                       $"as of date {options.AsOfDate} ");

                return ExportResult.Success(configuration.FileName, entities.Length);
            }
            catch (Exception e)
            {
                var message = $"Error in exporting {typeof(T)} to target location {options.Targets.ToJson()}";
                _logger.LogError(message, e);

                return ExportResult.Fail($"{message}. Reason: {e.Message}");
            }
        }
    }
}