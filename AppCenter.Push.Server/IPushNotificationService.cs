using System.Collections.Generic;
using System.Threading.Tasks;
using AppCenter.Push.Server.Messages;

namespace AppCenter.Push.Server
{
    public interface IPushNotificationService
    {
        Task<IEnumerable<AppCenterPushResponse>> SendPushNotificationAsync(AppCenterPushMessage appCenterPushMessage);

        Task<IEnumerable<NotificationOverviewResult>> GetPushNotificationsAsync(int top = 30);
    }
}