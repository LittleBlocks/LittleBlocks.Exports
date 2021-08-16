using System;
using System.Collections.Generic;
using System.Linq;

namespace Easify.Exports.Storage
{
    public sealed class CsvStorageTargetResolver : ICsvStorageTargetResolver
    {
        private readonly Dictionary<StorageTargetType, ICsvStorageTarget> _exports;

        public CsvStorageTargetResolver(IEnumerable<ICsvStorageTarget> exports)
        {
            _exports = exports?.ToDictionary(e => e.StorageTargetType) ?? throw new ArgumentNullException(nameof(exports));
        }
        
        public ICsvStorageTarget Resolve(StorageTargetType storageTargetType)
        {
            return _exports.ContainsKey(storageTargetType) ? _exports[storageTargetType] : null;
        }
    }
}