using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

namespace AppCenter.Push.Server.Messages
{
    [JsonObject]
    [DebuggerDisplay("AppCenterPushContent: {this.Name}")]
    public class AppCenterPushContent
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("custom_data")]
        public Dictionary<string, string> CustomData { get; set; } = new Dictionary<string, string>();
    }
}