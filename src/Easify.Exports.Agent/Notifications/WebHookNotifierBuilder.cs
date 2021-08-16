using Easify.Exports.Common;

namespace Easify.Exports.Agent.Notifications
{
    public class WebHookNotifierBuilder : IReportNotifierBuilder
    {
        public IReportNotifier NotificationFor<T>(string url, T t) where T : IExportNotification
        {
            return new WebHookReportNotifier(url, t);
        }
    }
}