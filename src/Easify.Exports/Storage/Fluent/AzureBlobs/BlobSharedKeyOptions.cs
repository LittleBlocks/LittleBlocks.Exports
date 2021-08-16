using System;

namespace Easify.Exports.Storage.Fluent.AzureBlobs
{
    public class BlobSharedKeyOptions : IHaveSharedKeyAccount, IAuthenticateWithSharedKey
    {
        public string Name { get; set; }
        public string Key { get; set; }

        public IAuthenticateWithSharedKey ForAccount(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            return this;
        }

        public void WithSharedKey(string key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }
    }
}