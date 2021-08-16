using System;

namespace Easify.Exports.Client.Exceptions
{
    public class InvalidUrlFormatException : Exception
    {
        public InvalidUrlFormatException(string message) : base(message)
        {
            
        }
    }
}