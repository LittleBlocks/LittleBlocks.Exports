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
using System.Linq;
using CsvHelper;
using CsvHelper.TypeConversion;

namespace LittleBlocks.Exports.Csv
{
    public static class CvsContextExtensions
    {
        public static void SetupFromCsvConfiguration(this CsvContext context, CsvExportConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(configuration);

            configuration.ClassMaps.ToList().ForEach(cm => context.RegisterClassMap(cm));

            var typeConverterOptions = new TypeConverterOptions
                {Formats = new[] {configuration.DateTimeFormat ?? ExporterDefaults.DefaultDateTimeFormat}};
            context.TypeConverterOptionsCache.AddOptions<DateTime>(typeConverterOptions);
            context.TypeConverterOptionsCache.AddOptions<DateTime?>(typeConverterOptions);
        }
    }
}