using System;

namespace Easify.Exports.Common
{
    public sealed class ExportExecutionResult
    {
        public ExportExecutionResult(Guid exportId, Guid exportExecutionId)
        {
            ExportId = exportId;
            ExportExecutionId = exportExecutionId;
        }
        public Guid ExportId { get; }
        public Guid ExportExecutionId { get; }
    }
}