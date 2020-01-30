using System.Collections.Generic;
using Newtonsoft.Json;

namespace AppCenter.Push.Server.Messages
{
    public class AppCenterPushDevicesTarget : AppCenterPushTarget
    {
        public override string Type => "devices_target";

        [JsonProperty("devices")]
        public IList<string> Devices { get; set; } = new List<string>();
    }
}