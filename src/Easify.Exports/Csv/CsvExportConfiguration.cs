using System;
using CsvHelper.Configuration;
using Easify.Exports.Storage;

namespace Easify.Exports.Csv
{
    public class CsvExportConfiguration
    {
        public StorageTarget[] Targets { get; set; } = { };
        public string FileName { get; set; }
        public CsvConfiguration Configuration { get; set; }

        public Type[] ClassMaps { get; set; } = { };
        public string DateTimeFormat { get; set; }
    }
}