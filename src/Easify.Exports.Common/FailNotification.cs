// This software is part of the Easify.Exports Library
// Copyright (C) 2021 Intermediate Capital Group
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

namespace Easify.Exports.Common
{
    public class FailNotification : IExportNotification
    {
        public FailNotification(Guid exportId, Guid exportExecutionId, string error)
        {
            ExportId = exportId;
            ExportExecutionId = exportExecutionId;
            Error = error ?? throw new ArgumentNullException(nameof(error));
        }

        public Guid ExportId { get; }
        public Guid ExportExecutionId { get; }
        public string Error { get; }

        public static FailNotification From(ExportExecutionContext context, string error)
        {
            return new(context.ExportId, context.ExportExecutionId, error);
        }
    }
}