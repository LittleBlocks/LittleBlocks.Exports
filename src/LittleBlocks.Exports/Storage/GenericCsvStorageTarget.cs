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
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Storage.Net.Blobs;

namespace LittleBlocks.Exports.Storage
{
    public sealed class GenericCsvStorageTarget : ICsvStorageTarget
    {
        private readonly IBlobStorage _blobStorage;

        public GenericCsvStorageTarget(StorageTargetType storageTargetType, IBlobStorage blobStorage)
        {
            if (!Enum.IsDefined(typeof(StorageTargetType), storageTargetType))
                throw new InvalidEnumArgumentException(nameof(storageTargetType), (int) storageTargetType,
                    typeof(StorageTargetType));

            StorageTargetType = storageTargetType;
            _blobStorage = blobStorage ?? throw new ArgumentNullException(nameof(blobStorage));
        }

        public StorageTargetType StorageTargetType { get; }

        public async Task WriteAsync(string targetLocation, string fileName, byte[] fileContent)
        {
            if (targetLocation == null) throw new ArgumentNullException(nameof(targetLocation));
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            if (fileContent == null) throw new ArgumentNullException(nameof(fileContent));

            var filePath = Path.Combine(targetLocation, fileName);
            await _blobStorage.WriteAsync(filePath, fileContent);
        }

        public Task<bool> ExistsAsync(string actualTargetFile)
        {
            return _blobStorage.ExistsAsync(actualTargetFile);
        }
    }
}