namespace Easify.Exports.Storage.Fluent.AzureBlobs
{
    public interface IHaveSharedKeyAccount
    {
        IAuthenticateWithSharedKey ForAccount(string name);
    }
}