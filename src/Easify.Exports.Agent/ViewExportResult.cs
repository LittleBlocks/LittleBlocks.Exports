using System.Collections.Generic;

namespace Easify.Exports.Agent
{
    public class ViewExportResult<T>
    {
        public string Schema { get; set; }
        public string ViewName { get; set; }
        public List<T> Data { get; set; }
    }
}
