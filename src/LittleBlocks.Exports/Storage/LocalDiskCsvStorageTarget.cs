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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Storage.Net;
using Storage.Net.Blobs;

namespace LittleBlocks.Exports.Storage
{
    [ExcludeFromCodeCoverage]
    public sealed class LocalDiskCsvStorageTarget : ICsvStorageTarget
    {
        public StorageTargetType StorageTargetType => StorageTargetType.LocalDisk;

        public async Task WriteAsync(string targetLocation, string fileName, byte[] fileContent)
        {
            ArgumentNullException.ThrowIfNull(targetLocation);
            ArgumentNullException.ThrowIfNull(fileName);
            ArgumentNullException.ThrowIfNull(fileContent);

            var filePath = Path.Combine(targetLocation, fileName);
            var blobStorage = CreateBlobStorage(targetLocation);

            await blobStorage.WriteAsync(filePath, fileContent);
        }

        public Task<bool> ExistsAsync(string actualTargetFile)
        {
            var blobStorage = CreateBlobStorage(Path.GetDirectoryName(actualTargetFile));

            return blobStorage.ExistsAsync(actualTargetFile);
        }

        private static IBlobStorage CreateBlobStorage(string basePath)
        {
            return StorageFactory.Blobs.DirectoryFiles(basePath);
        }
    }
}