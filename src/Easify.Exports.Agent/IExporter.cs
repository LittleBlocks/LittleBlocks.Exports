using System.Threading.Tasks;
using Easify.Exports.Common;
using Easify.Exports.Storage;

namespace Easify.Exports.Agent
{
    public interface IExporter
    {
        Task RunAsync(ExportExecutionContext executionContext, StorageTarget[] storageTargets);
    }
}