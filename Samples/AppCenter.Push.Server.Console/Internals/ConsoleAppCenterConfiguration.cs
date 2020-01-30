using System.Collections.Generic;

namespace AppCenter.Push.Server.Console.Internals
{
    internal class ConsoleAppCenterConfiguration : IAppCenterConfiguration
    {
        public ConsoleAppCenterConfiguration()
        {
            this.AppNames = new Dictionary<RuntimePlatform, string>();
        }

        public string OrganizationName { get; set; }

        public IDictionary<RuntimePlatform, string> AppNames { get; set; }

        public string ApiToken { get; set; }
    }
}