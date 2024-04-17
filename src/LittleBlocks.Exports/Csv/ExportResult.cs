// This software is part of the LittleBlocks.Exports Library
// Copyright (C) 2021 LittleBlocks
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System;

namespace LittleBlocks.Exports.Csv
{
    public sealed class ExportResult
    {
        private ExportResult()
        {
        }

        public bool HasError { get; private set; }
        public string Error { get; private set; }
        public string TargetFile { get; private set; }
        public int RecordCount { get; private set; }

        public static ExportResult Fail(string error, string targetFile = null)
        {
            ArgumentNullException.ThrowIfNull(error);

            return new ExportResult
            {
                HasError = true,
                Error = error,
                TargetFile = targetFile
            };
        }

        public static ExportResult Success(string targetFile, int recordCount)
        {
            ArgumentNullException.ThrowIfNull(targetFile);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(recordCount);

            return new ExportResult
            {
                HasError = false,
                TargetFile = targetFile,
                RecordCount = recordCount
            };
        }
    }
}