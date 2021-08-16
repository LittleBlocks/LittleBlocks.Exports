using System;

namespace Easify.Exports.Common
{
    public class SuccessNotification : IExportNotification
    {
        public SuccessNotification(Guid exportId, Guid exportExecutionId, int numberOfRecords, long duration)
        {
            ExportId = exportId;
            ExportExecutionId = exportExecutionId;
            NumberOfRecords = numberOfRecords;
            Duration = duration;
        }

        public Guid ExportId { get; }
        public Guid ExportExecutionId { get; }
        public int NumberOfRecords { get; }
        public long Duration { get; }

        public static SuccessNotification From(ExportExecutionContext context, int numberOfRecords, long duration)
        {
            return new SuccessNotification(context.ExportId, context.ExportExecutionId, numberOfRecords, duration);
        }
    }
}