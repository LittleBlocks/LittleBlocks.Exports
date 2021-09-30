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

namespace Easify.Exports.Storage.Fluent.AzureBlobs
{
    public class BlobAzureAdOptions : IHaveAzureAdAccount, IAuthenticateWithAzureAd, IAmInTenant
    {
        public string Name { get; set; }
        public string TenantId { get; set; }
        public string ApplicationId { get; set; }
        public string ApplicationSecret { get; set; }

        public void InTenant(string tenantId)
        {
            TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        }

        public IAmInTenant WithApplication(string applicationId, string applicationSecret)
        {
            ApplicationId = applicationId ?? throw new ArgumentNullException(nameof(applicationId));
            ApplicationSecret = applicationSecret ?? throw new ArgumentNullException(nameof(applicationSecret));

            return this;
        }

        public IAuthenticateWithAzureAd ForAccount(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            return this;
        }
    }
}