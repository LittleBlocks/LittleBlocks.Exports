using System;
using System.Threading.Tasks;
using Easify.Exports.Csv;
using Easify.Exports.Storage;

namespace Easify.Exports.Agent
{
    public interface IGroupItemExporter
    {
        Type GroupItemType { get; }
        Task<ExportResult> RunAsync(ExporterOptions options, StorageTarget[] storageTargets);
    }
}