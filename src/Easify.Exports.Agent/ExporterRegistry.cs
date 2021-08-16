using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Easify.Exports.Client.Exceptions;
using Easify.Exports.Common;

namespace Easify.Exports.Agent
{
    public class ExporterRegistry : IExporterRegistry, IExporterBuilder
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly ConcurrentDictionary<Guid, (Type Type, ExportMetadata Metadata)> _registry =
            new ConcurrentDictionary<Guid, (Type Type, ExportMetadata Metadata)>();

        public ExporterRegistry(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
            
        public void Register<T>(ExportMetadata exportMetadata) where T : IExporter
        {
            var key = exportMetadata.ExportId;
            var data = (Type: typeof(T), Metadata: exportMetadata);
            
            _registry.AddOrUpdate(key, data, (k, m) => data);
        }

        public IEnumerable<ExportMetadata> GetRegistrations()
        {
            return _registry.Values.Select(r => r.Metadata).ToArray();
        }

        public IExporter Build(Guid exporterId)
        {
            if (_registry.TryGetValue(exporterId, out var value))
                return _serviceProvider.GetService(value.Type) as IExporter;

            throw new ExporterNotFoundException($"No valid exporter was found for export with id: {exporterId}");
        }
    }
}