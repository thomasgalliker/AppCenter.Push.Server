using Newtonsoft.Json;

namespace AppCenter.Push.Server.Messages
{
    public enum NotificationState
    {
        Unknown,
        Processing,
        Enqueued,
        Completed,
        Cancelled
    }
}