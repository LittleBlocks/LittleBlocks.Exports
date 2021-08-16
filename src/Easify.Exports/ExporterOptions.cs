using System;
using Easify.Exports.Csv;
using Easify.Exports.Storage;

namespace Easify.Exports
{
    public sealed class ExporterOptions
    {
        public ExporterOptions(DateTimeOffset asOfDate, StorageTarget[] targets, string exportFilePrefix = "")
        {
            AsOfDate = asOfDate;
            Targets = targets ?? throw new ArgumentNullException(nameof(targets));
            ExportFilePrefix = exportFilePrefix;
        }

        public DateTimeOffset AsOfDate { get; }
        public StorageTarget[] Targets { get; }
        public string ExportFilePrefix { get; }

        public string DateTimeFormat { get; set; } = ExporterDefaults.DefaultDateTimeFormat;
        public string ColumnDelimiter { get; set; } = ExporterDefaults.DefaultColumnDelimiter;
        public string FileNameDateTimeFormat { get; set; } = ExporterDefaults.DefaultFileNameDateTimeFormat;
        
        public IExportFileNameBuilder CustomFileNameBuilder { get; set; }
    }
}