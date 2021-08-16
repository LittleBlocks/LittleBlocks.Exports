using System;

namespace Easify.Exports.Client.Exceptions
{
    public class ExporterNotFoundException : Exception
    {
        public ExporterNotFoundException(string message) : base(message)
        {
        }
    }
}