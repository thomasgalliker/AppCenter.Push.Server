using System.Collections.Generic;

namespace AppCenter.Push.Server
{
    public interface IAppCenterConfiguration
    {
        string OrganizationName { get; }

        IDictionary<RuntimePlatform, string> AppNames { get; }

        string ApiToken { get; }
    }
}