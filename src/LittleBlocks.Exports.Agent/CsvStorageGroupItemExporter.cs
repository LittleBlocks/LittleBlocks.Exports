// This software is part of the LittleBlocks.Exports Library
// Copyright (C) 2021 LittleBlocks
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
using LittleBlocks.Exports.Csv;
using LittleBlocks.Exports.Extensions;
using LittleBlocks.Exports.Storage;
using Microsoft.Extensions.Logging;

namespace LittleBlocks.Exports.Agent
{
    public abstract class CsvStorageGroupItemExporter<T> : IGroupItemExporter where T : class
    {
        private readonly IFileExporter _fileExporter;
        private readonly ILogger<CsvStorageGroupItemExporter<T>> _logger;

        protected CsvStorageGroupItemExporter(IFileExporter fileExporter,
            ILogger<CsvStorageGroupItemExporter<T>> logger)
        {
            _fileExporter = fileExporter ?? throw new ArgumentNullException(nameof(fileExporter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected abstract string ExportFilePrefix { get; }

        public Type GroupItemType => typeof(T);

        public async Task<ExportResult> RunAsync(ExporterOptions options, StorageTarget[] storageTargets)
        {
            _logger.LogInformation($"Loading the list of {typeof(T)}. export context: {options.ToJson()}");

            var data = await PrepareDataAsync(options);
            if (data == null)
                return ExportResult.Fail("Invalid data from the source.", ExportFilePrefix);

            var enumerable = data as T[] ?? data.ToArray();
            _logger.LogInformation(
                $"Exporting {enumerable.Length} {typeof(T)} in the list. export context: {options.ToJson()}");

            var newOptions = CreateExporterOptions(options) ?? options;

            return await _fileExporter.ExportAsync(enumerable, newOptions);
        }

        protected abstract Task<IEnumerable<T>> PrepareDataAsync(ExporterOptions options);

        protected virtual ExporterOptions CreateExporterOptions(ExporterOptions exportOptions)
        {
            return exportOptions;
        }
    }
}