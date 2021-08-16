using Easify.Exports.Common;

namespace Easify.Exports.Agent.Notifications
{
    public interface IReportNotifierBuilder
    {
        IReportNotifier NotificationFor<T>(string url, T t) where T : IExportNotification;
    }
}