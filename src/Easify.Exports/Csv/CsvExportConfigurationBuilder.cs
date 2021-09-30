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
using System.Globalization;
using CsvHelper.Configuration;
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
            _exportFileNameBuilder =
                exportFileNameBuilder ?? throw new ArgumentNullException(nameof(exportFileNameBuilder));
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
                ClassMaps = new[] {classMap},
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