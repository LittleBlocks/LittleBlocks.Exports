namespace Easify.Exports.Storage.Fluent.AzureBlobs
{
    public interface IAuthenticateWithAzureAd : IAmInTenant
    {
        IAmInTenant WithApplication(string applicationId, string applicationSecret);
    }
}