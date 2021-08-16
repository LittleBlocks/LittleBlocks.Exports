using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Easify.Exports.Csv;
using Easify.Exports.Extensions;
using Easify.Exports.Storage;
using Microsoft.Extensions.Logging;

namespace Easify.Exports.Agent
{
    public abstract class CsvStorageGroupViewExporter<T> : IGroupItemExporter where T : class
    {
        private readonly IFileExporter _fileExporter;
        private readonly ILogger<CsvStorageGroupViewExporter<T>> _logger;

        protected CsvStorageGroupViewExporter(IFileExporter fileExporter, ILogger<CsvStorageGroupViewExporter<T>> logger)
        {
            _fileExporter = fileExporter ?? throw new ArgumentNullException(nameof(fileExporter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected abstract string[] Schemas { get; }

        protected abstract string ViewPrefix { get; }

        public Type GroupItemType => typeof(T);

        public async Task<ExportResult> RunAsync(ExporterOptions options, StorageTarget[] storageTargets)
        {
            _logger.LogInformation($"Loading the list of {typeof(T)}. export context: {options.ToJson()}");

            var viewName = typeof(T).Name;

            var results = new List<ViewExportResult<T>>();

            foreach (var schema in Schemas)
            {
                var result = await PrepareDataAsync(ViewPrefix, schema, viewName, options);
                if(result?.Data != null) results.Add(result);
            }

            if (results.Count == 0)
                return ExportResult.Fail("Invalid data from the source.", viewName);

            var fileExportResults = new List<ExportResult>();

            foreach (var result in results)
            {
                var enumerable = result as T[] ?? result.Data.ToArray();
                
                _logger.LogInformation($"Exporting {enumerable.Length} {typeof(T)} in the list. export context: {options.ToJson()}");
                
                fileExportResults.Add(await _fileExporter.ExportAsync(enumerable, CreateExporterOptions(options, result) ?? options));
            }

            return fileExportResults.Any(er => er.HasError) 
                ? ExportResult.Fail("Invalid data from the source.", viewName) 
                : ExportResult.Success("", results.Count);
        }

        protected abstract Task<ViewExportResult<T>> PrepareDataAsync(string viewPrefix, string schema, string viewName, ExporterOptions options);

        protected virtual ExporterOptions CreateExporterOptions(ExporterOptions exportOptions, ViewExportResult<T> viewName)
        {
            return exportOptions;
        }
    }
}
