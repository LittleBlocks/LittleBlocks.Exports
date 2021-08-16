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
    public sealed class ExportMetadata
    {
        public const string DefaultCronSchedule = "0 6 * * *";

        public ExportMetadata(Guid exportId, string exportName, string exportDescription,
            string defaultExportSchedule = DefaultCronSchedule)
        {
            ExportId = exportId;
            ExportName = exportName ?? throw new ArgumentNullException(nameof(exportName));
            ExportDescription = exportDescription ?? throw new ArgumentNullException(nameof(exportDescription));
            DefaultExportSchedule =
                defaultExportSchedule ?? throw new ArgumentNullException(nameof(defaultExportSchedule));
        }

        public Guid ExportId { get; }
        public string ExportName { get; }
        public string ExportDescription { get; }
        public string DefaultExportSchedule { get; }
    }
}