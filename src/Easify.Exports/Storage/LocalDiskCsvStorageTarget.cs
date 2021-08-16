using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Storage.Net;
using Storage.Net.Blobs;

namespace Easify.Exports.Storage
{
    [ExcludeFromCodeCoverage]
    public sealed class LocalDiskCsvStorageTarget : ICsvStorageTarget
    {
        public StorageTargetType StorageTargetType => StorageTargetType.LocalDisk;

        public async Task WriteAsync(string targetLocation, string fileName, byte[] fileContent)
        {
            if (targetLocation == null) throw new ArgumentNullException(nameof(targetLocation));
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            if (fileContent == null) throw new ArgumentNullException(nameof(fileContent));

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