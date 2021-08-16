using System.Collections.Generic;
using System.Threading.Tasks;
using Easify.Exports.Csv;

namespace Easify.Exports
{
    public interface IFileExporter
    {
        Task<ExportResult> ExportAsync<T>(IEnumerable<T> entities, ExporterOptions options) where T : class;
    }
}