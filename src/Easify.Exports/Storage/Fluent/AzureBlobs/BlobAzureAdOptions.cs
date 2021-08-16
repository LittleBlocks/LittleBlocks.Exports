using System;

namespace Easify.Exports.Storage.Fluent.AzureBlobs
{
    public class BlobAzureAdOptions : IHaveAzureAdAccount, IAuthenticateWithAzureAd, IAmInTenant
    {
        public string Name { get; set; }
        public string TenantId { get; set; }
        public string ApplicationId { get; set; }
        public string ApplicationSecret { get; set; }

        public IAuthenticateWithAzureAd ForAccount(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            return this;
        }        
        
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
    }
}