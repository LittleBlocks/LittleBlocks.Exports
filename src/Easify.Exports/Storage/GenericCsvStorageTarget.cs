using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Storage.Net.Blobs;

namespace Easify.Exports.Storage
{
    public sealed class GenericCsvStorageTarget : ICsvStorageTarget
    {
        private readonly IBlobStorage _blobStorage;
        public StorageTargetType StorageTargetType { get; }

        public GenericCsvStorageTarget(StorageTargetType storageTargetType, IBlobStorage blobStorage)
        {
            if (!Enum.IsDefined(typeof(StorageTargetType), storageTargetType))
                throw new InvalidEnumArgumentException(nameof(storageTargetType), (int) storageTargetType, typeof(StorageTargetType));

            StorageTargetType = storageTargetType;
            _blobStorage = blobStorage ?? throw new ArgumentNullException(nameof(blobStorage));
        }

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