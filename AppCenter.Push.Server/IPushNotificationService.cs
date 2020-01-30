using System.Collections.Generic;
using System.Threading.Tasks;
using AppCenter.Push.Server.Messages;
using AppCenter.Push.Server.Model;

namespace AppCenter.Push.Server
{
    public interface IPushNotificationService
    {
        Task<IEnumerable<AppCenterPushResponse>> SendPushNotificationAsync(AppCenterPushMessage appCenterPushMessage);
    }
}