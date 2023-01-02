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
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;

namespace LittleBlocks.Exports.Storage.Fluent.S3
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