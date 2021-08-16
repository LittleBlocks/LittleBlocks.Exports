using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easify.Exports.Csv
{
    public class StorageTargetException : Exception
    {
        public IEnumerable<Exception> Exceptions { get; }

        public StorageTargetException(IEnumerable<Exception> exceptions) : base(FormatMessage(exceptions))
        {
            Exceptions = exceptions ?? throw new ArgumentNullException(nameof(exceptions));
        }

        private static string FormatMessage(IEnumerable<Exception> exceptions)
        {
            var builder = new StringBuilder("Error in writing the file to multiple storage. ");
            builder.Append(string.Join(Environment.NewLine, exceptions.Select(e => e.Message).ToArray()));

            return builder.ToString();
        }
    }
}