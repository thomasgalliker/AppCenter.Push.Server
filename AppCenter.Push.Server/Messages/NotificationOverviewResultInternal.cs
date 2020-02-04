using System.Collections.Generic;
using Newtonsoft.Json;

namespace AppCenter.Push.Server.Messages
{
    [JsonObject]
    internal class NotificationOverviewResultInternal
    {
        public NotificationOverviewResultInternal()
        {
            this.Values = new List<NotificationOverviewResult>();
        }

        [JsonProperty(PropertyName = "values")]
        public IEnumerable<NotificationOverviewResult> Values { get; set; }
    }
}