using System;

namespace Easify.Exports.Csv
{
    public sealed class ExportResult
    {
        private ExportResult()
        {
            
        }
        
        public static ExportResult Fail(string error, string targetFile = null)
        {
            if (error == null) throw new ArgumentNullException(nameof(error));
            
            return new ExportResult
            {
                HasError = true,
                Error = error,
                TargetFile = targetFile
            };
        }

        public static ExportResult Success(string targetFile, int recordCount)
        {
            if (targetFile == null) throw new ArgumentNullException(nameof(targetFile));
            if (recordCount <= 0) throw new ArgumentOutOfRangeException(nameof(recordCount));
            
            return new ExportResult
            {
                HasError = false,
                TargetFile = targetFile,
                RecordCount = recordCount
            };
        }

        public bool HasError { get; private set; }
        public string Error { get; private set; }
        public string TargetFile { get; private set; }
        public int RecordCount { get; private set; }
    }
}