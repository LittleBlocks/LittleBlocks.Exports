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

using System.Collections.Generic;
using System.Threading.Tasks;
using LittleBlocks.Exports.Common;
using LittleBlocks.RestEase.Client;
using RestEase;

namespace LittleBlocks.Exports.Client
{
    public interface IExporterClient : IRestClient
    {
        [Post("api/v1/exporters/new")]
        Task<ExportExecutionResult> ExecuteExportAsync([Body] ExportExecutionContext executionContext);

        [Get("api/v1/exporters/discover")]
        Task<IEnumerable<ExportMetadata>> DiscoverExportsAsync();
    }
}