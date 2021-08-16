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
using Easify.Exports.Csv;
using Easify.Exports.Storage;

namespace Easify.Exports
{
    public sealed class ExporterOptions
    {
        public ExporterOptions(DateTimeOffset asOfDate, StorageTarget[] targets, string exportFilePrefix = "")
        {
            AsOfDate = asOfDate;
            Targets = targets ?? throw new ArgumentNullException(nameof(targets));
            ExportFilePrefix = exportFilePrefix;
        }

        public DateTimeOffset AsOfDate { get; }
        public StorageTarget[] Targets { get; }
        public string ExportFilePrefix { get; }

        public string DateTimeFormat { get; set; } = ExporterDefaults.DefaultDateTimeFormat;
        public string ColumnDelimiter { get; set; } = ExporterDefaults.DefaultColumnDelimiter;
        public string FileNameDateTimeFormat { get; set; } = ExporterDefaults.DefaultFileNameDateTimeFormat;

        public IExportFileNameBuilder CustomFileNameBuilder { get; set; }
    }
}