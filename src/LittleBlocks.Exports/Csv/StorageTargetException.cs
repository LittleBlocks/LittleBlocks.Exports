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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LittleBlocks.Exports.Csv
{
    public class StorageTargetException(IEnumerable<Exception> exceptions) : Exception(FormatMessage(exceptions))
    {
        public IEnumerable<Exception> Exceptions { get; } = exceptions ?? throw new ArgumentNullException(nameof(exceptions));

        private static string FormatMessage(IEnumerable<Exception> exceptions)
        {
            var builder = new StringBuilder("Error in writing the file to multiple storage. ");
            builder.Append(string.Join(Environment.NewLine, exceptions.Select(e => e.Message).ToArray()));

            return builder.ToString();
        }
    }
}