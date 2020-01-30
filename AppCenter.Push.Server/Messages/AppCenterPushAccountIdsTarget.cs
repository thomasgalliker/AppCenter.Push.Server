using System.Collections.Generic;
using Newtonsoft.Json;

namespace AppCenter.Push.Server.Messages
{
    public class AppCenterPushAccountIdsTarget : AppCenterPushTarget
    {
        public override string Type => "account_ids_target";

        [JsonProperty("account_ids")]
        public IList<string> AccountIds { get; set; } = new List<string>();
    }
}