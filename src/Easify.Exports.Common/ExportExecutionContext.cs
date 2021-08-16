using System;

namespace Easify.Exports.Common
{
    public sealed class ExportExecutionContext
    {
        public ExportExecutionContext(Guid exportId, Guid exportExecutionId, DateTimeOffset asOfDate, string successWebHook, string failWebHook)
        {
            ExportId = exportId;
            ExportExecutionId = exportExecutionId;
            AsOfDate = asOfDate;
            SuccessWebHook = successWebHook ?? throw new ArgumentNullException(nameof(successWebHook));
            FailWebHook = failWebHook ?? throw new ArgumentNullException(nameof(failWebHook));
        }

        public Guid ExportId { get; }
        public Guid ExportExecutionId { get; }
        public DateTimeOffset AsOfDate { get; }
        public string SuccessWebHook { get; }
        public string FailWebHook { get; }
    }
}