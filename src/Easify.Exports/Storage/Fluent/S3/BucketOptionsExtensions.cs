using System;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;

namespace Easify.Exports.Storage.Fluent.S3
{
    public static class BucketOptionsExtensions
    {
        public static AWSCredentials CreateOrRefreshSamlCredentials(this BucketOptions bucketOptions)
        {
            if (bucketOptions == null) throw new ArgumentNullException(nameof(bucketOptions));
            
            CreateSamlEndpoint(bucketOptions);
            CreateOrUpdateProfile(bucketOptions);

            return CreateCredentials(bucketOptions);
        }

        private static AWSCredentials CreateCredentials(BucketOptions bucketOptions)
        {
            var chain = new CredentialProfileStoreChain();
            if (chain.TryGetAWSCredentials(bucketOptions.Profile, out var credentials))
                return credentials;

            throw new InvalidAwsProfileException($"The profile {bucketOptions.Profile} is not available in cache");
        }

        private static void CreateOrUpdateProfile(BucketOptions bucketOptions)
        {
            var options = new CredentialProfileOptions
            {
                EndpointName = bucketOptions.EndpointName,
                RoleArn = bucketOptions.RoleArn
            };

            var profile = new CredentialProfile(bucketOptions.Profile, options);

            var credentialsFile = new NetSDKCredentialsFile();
            credentialsFile.RegisterProfile(profile);
        }

        private static void CreateSamlEndpoint(BucketOptions bucketOptions)
        {
            var endpoint = new SAMLEndpoint(bucketOptions.EndpointName, new Uri(bucketOptions.EndpointUrl),
                SAMLAuthenticationType.NTLM);
            var manager = new SAMLEndpointManager();
            manager.RegisterEndpoint(endpoint);
        }
    }
}