using System;

namespace Easify.Exports.Common
{
    public class FailNotification : IExportNotification
    {
        public FailNotification(Guid exportId, Guid exportExecutionId, string error)
        {
            ExportId = exportId;
            ExportExecutionId = exportExecutionId;
            Error = error ?? throw new ArgumentNullException(nameof(error));
        }

        public Guid ExportId { get; }
        public Guid ExportExecutionId { get; }
        public string Error { get; }

        public static FailNotification From(ExportExecutionContext context, string error)
        {
            return new FailNotification(context.ExportId, context.ExportExecutionId, error);
        }
    }
}