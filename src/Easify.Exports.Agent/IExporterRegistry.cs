using System.Collections.Generic;
using Easify.Exports.Common;

 namespace Easify.Exports.Agent
{
    public interface IExporterRegistry
    {
        void Register<T>(ExportMetadata exportMetadata) where T : IExporter;
        IEnumerable<ExportMetadata> GetRegistrations();
    }
}