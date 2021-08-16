using System.Collections.Generic;
using System.Threading.Tasks;

namespace Easify.Exports.Common
{
    public interface IExporterController
    {
        Task<ExportExecutionResult> ExecuteExportAsync(ExportExecutionContext executionContext);
        Task<IEnumerable<ExportMetadata>> DiscoverExportsAsync();
    }
}