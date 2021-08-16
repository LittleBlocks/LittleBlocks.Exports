// This software is part of the Easify.Exports Library
// Copyright (C) 2021 Intermediate Capital Group
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

        public INeedBucket InRegion(string region)
        {
            Region = region ?? throw new ArgumentNullException(nameof(region));
            return this;
        }

        public void ForBucket(string bucketName)
        {
            BucketName = bucketName ?? throw new ArgumentNullException(nameof(bucketName));
        }

        public IAmInRegion WithProfile(string profile)
        {
            Profile = profile ?? throw new ArgumentNullException(nameof(profile));
            return this;
        }
    }
}