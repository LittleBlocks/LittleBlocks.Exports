using System;
using System.Globalization;
using System.IO;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Microsoft.Extensions.Logging;

namespace Easify.Exports.Csv
{
    public class CsvExportConfigurationBuilder : ICsvExportConfigurationBuilder
    {
        private readonly ICsvContextMapResolver _csvContextMapResolver;
        private readonly IExportFileNameBuilder _exportFileNameBuilder;
        private readonly ILogger<CsvExportConfigurationBuilder> _logger;

        public CsvExportConfigurationBuilder(ICsvContextMapResolver csvContextMapResolver,
            IExportFileNameBuilder exportFileNameBuilder, ILogger<CsvExportConfigurationBuilder> logger)
        {
            _csvContextMapResolver =
                csvContextMapResolver ?? throw new ArgumentNullException(nameof(csvContextMapResolver));
            _exportFileNameBuilder = exportFileNameBuilder ?? throw new ArgumentNullException(nameof(exportFileNameBuilder));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public CsvExportConfiguration Build<T>(ExporterOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            var classMap = _csvContextMapResolver.Resolve<T>();
            if (classMap == null)
            {
                _logger.LogWarning($"Unable to get map for type {typeof(T)} {nameof(classMap)}");
                return null;
            }

            var config = BuildCsvConfiguration(classMap, options);
            var fileNameBuilder = options.CustomFileNameBuilder ?? _exportFileNameBuilder;
            
            return new CsvExportConfiguration
            {
                Targets = options.Targets,
                FileName = fileNameBuilder.Build(options),
                Configuration = config,
                ClassMaps = new []{ classMap },
                DateTimeFormat = options.DateTimeFormat ?? ExporterDefaults.DefaultDateTimeFormat
            };
        }

        private static CsvConfiguration BuildCsvConfiguration(Type classMap, ExporterOptions options)
        {
            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = options.ColumnDelimiter ?? ExporterDefaults.DefaultColumnDelimiter,
                TrimOptions = TrimOptions.InsideQuotes,
                HasHeaderRecord = true
            };

            return configuration;
        }
    }
}