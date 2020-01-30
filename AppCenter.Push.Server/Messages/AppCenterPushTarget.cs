using System.Diagnostics;
using Newtonsoft.Json;

namespace AppCenter.Push.Server.Messages
{
    [DebuggerDisplay("AppCenterPushTarget: {this.Type}")]
    public abstract class AppCenterPushTarget
    {
        [JsonProperty("type")]
        public abstract string Type { get; }
    }
}