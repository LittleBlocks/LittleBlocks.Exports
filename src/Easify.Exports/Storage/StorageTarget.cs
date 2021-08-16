namespace Easify.Exports.Storage
{
    public sealed class StorageTarget
    {
        public string TargetLocation { get; set; }
        public StorageTargetType StorageTargetType { get; set; }
        public int RetryDelay { get; set; } = 5;
    }
}