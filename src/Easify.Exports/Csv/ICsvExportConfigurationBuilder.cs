using System;

namespace Easify.Exports.Csv
{
    public interface ICsvExportConfigurationBuilder
    {
        CsvExportConfiguration Build<T>(ExporterOptions context);
    }
}