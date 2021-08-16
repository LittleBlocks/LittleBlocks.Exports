using System;

namespace Easify.Exports.Client.Exceptions
{
    public class DuplicateExporterException : Exception
    {
        public DuplicateExporterException(string message) : base(message)
        {
        }
    }
}