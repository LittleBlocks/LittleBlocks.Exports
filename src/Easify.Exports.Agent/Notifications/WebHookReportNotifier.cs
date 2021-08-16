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
        private readonly string _url;
        private readonly IExportNotification _payload;

        public WebHookReportNotifier(string url, IExportNotification payload)
        {
            _url = url;
            _payload = payload;
        }
        
        public async Task RunAsync()
        {
            using (var client = new HttpClient())
            {
                var content = JsonConvert.SerializeObject(_payload);
                var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
                
                await client.PostAsync(_url, httpContent);
            }
        }
    }
}