using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Easify.Exports.Common;
using Easify.Exports.Csv;
using Easify.Exports.Extensions;
using Easify.Exports.Storage;
using Microsoft.Extensions.Logging;

namespace Easify.Exports.Agent
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

        public Type GroupItemType => typeof(T);
        protected abstract string ExportFilePrefix { get; }

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
