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

namespace LittleBlocks.Exports.Common
{
    public sealed class ExportExecutionContext
    {
        public ExportExecutionContext(Guid exportId, Guid exportExecutionId, DateTimeOffset asOfDate,
            string successWebHook, string failWebHook)
        {
            ExportId = exportId;
            ExportExecutionId = exportExecutionId;
            AsOfDate = asOfDate;
            SuccessWebHook = successWebHook ?? throw new ArgumentNullException(nameof(successWebHook));
            FailWebHook = failWebHook ?? throw new ArgumentNullException(nameof(failWebHook));
        }

        public Guid ExportId { get; }
        public Guid ExportExecutionId { get; }
        public DateTimeOffset AsOfDate { get; }
        public string SuccessWebHook { get; }
        public string FailWebHook { get; }
    }
}