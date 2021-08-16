namespace Easify.Exports.Storage.Fluent.S3.Fluent
{
    public interface IAmInRegion : INeedBucket
    {
        INeedBucket InRegion(string region);
    }
}