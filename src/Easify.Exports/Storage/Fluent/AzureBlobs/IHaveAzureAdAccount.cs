namespace Easify.Exports.Storage.Fluent.AzureBlobs
{
    public interface IHaveAzureAdAccount
    {
        IAuthenticateWithAzureAd ForAccount(string name);
    }
}