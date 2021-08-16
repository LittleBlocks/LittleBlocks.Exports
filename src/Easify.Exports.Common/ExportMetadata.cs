using System;

namespace Easify.Exports.Common
{
    public sealed class ExportMetadata
    {
        public const string DefaultCronSchedule = "0 6 * * *";

        public ExportMetadata(Guid exportId, string exportName, string exportDescription, string defaultExportSchedule = DefaultCronSchedule)
        {
            ExportId = exportId;
            ExportName = exportName ?? throw new ArgumentNullException(nameof(exportName));
            ExportDescription = exportDescription ?? throw new ArgumentNullException(nameof(exportDescription));
            DefaultExportSchedule = defaultExportSchedule ?? throw new ArgumentNullException(nameof(defaultExportSchedule));
        }

        public Guid ExportId { get; }
        public string ExportName { get; }
        public string ExportDescription { get; }
        public string DefaultExportSchedule { get; }
    }
}