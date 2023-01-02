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
using System.Linq;

namespace LittleBlocks.Exports.Storage
{
    public sealed class CsvStorageTargetResolver : ICsvStorageTargetResolver
    {
        private readonly Dictionary<StorageTargetType, ICsvStorageTarget> _exports;

        public CsvStorageTargetResolver(IEnumerable<ICsvStorageTarget> exports)
        {
            _exports = exports?.ToDictionary(e => e.StorageTargetType) ??
                       throw new ArgumentNullException(nameof(exports));
        }

        public ICsvStorageTarget Resolve(StorageTargetType storageTargetType)
        {
            return _exports.ContainsKey(storageTargetType) ? _exports[storageTargetType] : null;
        }
    }
}