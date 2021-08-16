using System;
using System.Collections.Generic;
using Easify.Exports.Client.Exceptions;
using Easify.RestEase;
using Easify.Exports.Common;
using RestEase;

namespace Easify.Exports.Client
{
    public sealed class ExporterClientBuilder : IExporterClientBuilder, IExporterClientRegistry
    {
        private readonly IDictionary<string, string> _exporterCache = new Dictionary<string, string>();

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

        public IExporterClient Build(string name)
        {
            if (!_exporterCache.ContainsKey(name))
                throw new ExporterNotFoundException($"No exporter found with name: {name}. The {name} should be added using AddClient in configuration");

            return RestClient.For<IExporterClient>(_exporterCache[name]);
            
            // TODO: Need to be replaced with this due to the issue
            // return _clientBuilder.Build<IExporterClient>(_exporterCache[name]);
        }
    }
}