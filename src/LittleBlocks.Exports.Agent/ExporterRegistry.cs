// This software is part of the LittleBlocks.Exports Library
// Copyright (C) 2021 LittleBlocks
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using LittleBlocks.Exports.Client.Exceptions;
using LittleBlocks.Exports.Common;

namespace LittleBlocks.Exports.Agent
{
    public class ExporterRegistry : IExporterRegistry, IExporterBuilder
    {
        private readonly ConcurrentDictionary<Guid, (Type Type, ExportMetadata Metadata)> _registry =
            new();

        private readonly IServiceProvider _serviceProvider;

        public ExporterRegistry(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IExporter Build(Guid exporterId)
        {
            if (_registry.TryGetValue(exporterId, out var value))
                return _serviceProvider.GetService(value.Type) as IExporter;

            throw new ExporterNotFoundException($"No valid exporter was found for export with id: {exporterId}");
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
    }
}