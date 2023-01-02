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
using System.Collections.Generic;
using LittleBlocks.Exports.Client.Exceptions;
using RestEase;

namespace LittleBlocks.Exports.Client
{
    public sealed class ExporterClientBuilder : IExporterClientBuilder, IExporterClientRegistry
    {
        private readonly IDictionary<string, string> _exporterCache = new Dictionary<string, string>();

        public IExporterClient Build(string name)
        {
            if (!_exporterCache.ContainsKey(name))
                throw new ExporterNotFoundException(
                    $"No exporter found with name: {name}. The {name} should be added using AddClient in configuration");

            return RestClient.For<IExporterClient>(_exporterCache[name]);

            // TODO: Need to be replaced with this due to the issue
            // return _clientBuilder.Build<IExporterClient>(_exporterCache[name]);
        }

        public IExporterClientRegistry AddClient(string name, string clientUrl)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (clientUrl == null) throw new ArgumentNullException(nameof(clientUrl));

            if (_exporterCache.ContainsKey(name))
                throw new DuplicateExporterException($"Duplicate exporter with name {name} exists in the cache");

            if (!Uri.IsWellFormedUriString(clientUrl, UriKind.Absolute))
                throw new InvalidUrlFormatException($"The url {clientUrl} is not wellFormed");

            _exporterCache.Add(name, clientUrl);
            return this;
        }
    }
}