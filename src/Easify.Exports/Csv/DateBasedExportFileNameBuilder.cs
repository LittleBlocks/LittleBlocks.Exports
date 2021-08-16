using System;

namespace Easify.Exports.Csv
{
    public class DateBasedExportFileNameBuilder : IExportFileNameBuilder
    {
        public string Build(ExporterOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            var format = options.FileNameDateTimeFormat ?? ExporterDefaults.DefaultFileNameDateTimeFormat;
            return $"{options.ExportFilePrefix}{options.AsOfDate.ToString(format)}.csv";
        }
    }
}