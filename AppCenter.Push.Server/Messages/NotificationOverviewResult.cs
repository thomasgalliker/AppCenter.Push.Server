using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace AppCenter.Push.Server.Messages
{
    [DebuggerDisplay("NotificationOverviewResult: NotificationId={this.NotificationId}")]
    public class NotificationOverviewResult
    {
        [JsonProperty(PropertyName = "notification_id")]
        public string NotificationId { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "notification_target")]
        [JsonConverter(typeof(AppCenterPushTargetJsonConverter))]
        public AppCenterPushTarget NotificationTarget { get; set; }

        [JsonProperty(PropertyName = "send_time")]
        public DateTime SendTime { get; set; }

        [JsonProperty(PropertyName = "pns_send_failure")]
        public int? PnsSendFailure { get; set; }

        [JsonProperty(PropertyName = "pns_send_success")]
        public int? PnsSendSuccess { get; set; }

        /// <summary>
        ///     Gets or sets state of the notification. Possible values include:
        ///     'Cancelled', 'Completed', 'Enqueued', 'Processing', 'Unknown'
        /// </summary>
        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        [JsonConverter(typeof(RuntimePlatformJsonConverter))]
        public RuntimePlatform RuntimePlatform { get; set; }
    }
}