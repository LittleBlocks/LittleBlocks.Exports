using System;

namespace Easify.Exports.Storage.Fluent.S3
{
    public class InvalidAwsProfileException : Exception
    {
        public InvalidAwsProfileException(string message): base(message)
        {
        }
    }
}