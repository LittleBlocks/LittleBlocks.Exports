using Easify.Exports.Storage;

namespace Easify.Exports
{
    public class ExporterMetadata
    {
        public string ExportId { get; set; }
        public string ExportName { get; set; }
        public string ExportDescription { get; set; }
        public string ExportSchedule { get; set; }

        public StorageTarget[] StorageTargets { get; set; } = { };
    }
}