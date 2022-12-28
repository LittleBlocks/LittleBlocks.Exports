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
    public class SuccessNotification : IExportNotification
    {
        public SuccessNotification(Guid exportId, Guid exportExecutionId, int numberOfRecords, long duration)
        {
            ExportId = exportId;
            ExportExecutionId = exportExecutionId;
            NumberOfRecords = numberOfRecords;
            Duration = duration;
        }

        public Guid ExportId { get; }
        public Guid ExportExecutionId { get; }
        public int NumberOfRecords { get; }
        public long Duration { get; }

        public static SuccessNotification From(ExportExecutionContext context, int numberOfRecords, long duration)
        {
            return new(context.ExportId, context.ExportExecutionId, numberOfRecords, duration);
        }
    }
}