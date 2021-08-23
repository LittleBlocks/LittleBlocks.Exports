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

using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Easify.Exports.Common;
using Newtonsoft.Json;

namespace Easify.Exports.Agent.Notifications
{
    [ExcludeFromCodeCoverage]
    public class WebHookReportNotifier : IReportNotifier
    {
        private readonly IExportNotification _payload;
        private readonly string _url;

        public WebHookReportNotifier(string url, IExportNotification payload)
        {
            _url = url;
            _payload = payload;
        }

        public async Task RunAsync()
        {
            using var client = new HttpClient();
            var content = JsonConvert.SerializeObject(_payload);
            var httpContent = new StringContent(content, Encoding.UTF8, "application/json");

            await client.PostAsync(_url, httpContent);
        }
    }
}