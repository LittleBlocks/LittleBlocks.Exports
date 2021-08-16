using System.Threading.Tasks;

namespace Easify.Exports.Storage
{
    public interface ICsvStorageTarget
    {
        StorageTargetType StorageTargetType { get; }
        Task WriteAsync(string targetLocation, string fileName, byte[] fileContent);
        Task<bool> ExistsAsync(string actualTargetFile);
    }
}