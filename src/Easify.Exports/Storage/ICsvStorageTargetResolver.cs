namespace Easify.Exports.Storage
{
    public interface ICsvStorageTargetResolver
    {
        ICsvStorageTarget Resolve(StorageTargetType storageTargetType);
    }
}