using System.Collections.Generic;
using System.Threading.Tasks;
using Easify.RestEase.Client;
using Easify.Exports.Common;
using RestEase;

namespace Easify.Exports.Client
{
    public interface IExporterClient : IRestClient
    {
        [Post("api/v1/exporters/new")]
        Task<ExportExecutionResult> ExecuteExportAsync([Body] ExportExecutionContext executionContext);
        
        [Get("api/v1/exporters/discover")]
        Task<IEnumerable<ExportMetadata>> DiscoverExportsAsync();
    }
}