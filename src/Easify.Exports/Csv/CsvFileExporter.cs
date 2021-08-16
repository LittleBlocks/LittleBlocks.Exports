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
        private readonly ICsvFileWriter _csvFileWriter;
        private readonly ICsvExportConfigurationBuilder _csvExportConfigurationBuilder;
        private readonly ILogger<CsvFileExporter> _logger;

        public CsvFileExporter(ICsvFileWriter csvFileWriter, ICsvExportConfigurationBuilder csvExportConfigurationBuilder,
            ILogger<CsvFileExporter> logger)
        {
            _csvFileWriter = csvFileWriter ?? throw new ArgumentNullException(nameof(csvFileWriter));
            _csvExportConfigurationBuilder = csvExportConfigurationBuilder ?? throw new ArgumentNullException(nameof(csvExportConfigurationBuilder));
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
                    var error = $"Error in creating export configuration '{typeof(T)}'. Make sure the ClassMap has been registered";
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