using System;
using Easify.Exports.Storage.Fluent.S3.Fluent;

namespace Easify.Exports.Storage.Fluent.S3
{
    public sealed class BucketOptions : IHaveProfile, IAmInRegion
    {
        public string Profile { get; set; }
        public string Region { get; set; }
        public string BucketName { get; set; }
        public string EndpointName { get; set; } = "SAML";
        public string RoleArn { get; set; }
        public string EndpointUrl { get; set; }

        public IAmInRegion WithProfile(string profile)
        {
            Profile = profile ?? throw new ArgumentNullException(nameof(profile));
            return this;
        }

        public INeedBucket InRegion(string region)
        {
            Region = region ?? throw new ArgumentNullException(nameof(region));
            return this;
        }

        public void ForBucket(string bucketName)
        {
            BucketName = bucketName ?? throw new ArgumentNullException(nameof(bucketName));
        }
    }
}