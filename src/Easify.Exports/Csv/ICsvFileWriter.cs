using System.Collections.Generic;
using System.Threading.Tasks;

namespace Easify.Exports.Csv
{
    public interface ICsvFileWriter
    {
        Task WriteFileAsync<T>(IEnumerable<T> items, CsvExportConfiguration configuration) where T : class;
    }
}