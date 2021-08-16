using System;

namespace Easify.Exports.Storage
{
    public sealed class StorageTargetNotFoundException : Exception
    {
        public StorageTargetNotFoundException(string message) : base(message) { }
    }
}