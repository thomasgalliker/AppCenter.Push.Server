using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AppCenter.Push.Server.Logging;
using AppCenter.Push.Server.Messages;
using AppCenter.Push.Server.Model;
using Newtonsoft.Json;

namespace AppCenter.Push.Server
{
    public class AppCenterPushNotificationService : IPushNotificationService
    {
        private readonly ILogger logger;
        private readonly HttpClient httpClient;
        private readonly IAppCenterConfiguration appCenterConfiguration;

        public AppCenterPushNotificationService(IAppCenterConfiguration appCenterConfiguration)
            : this(Logger.Current, new HttpClient(), appCenterConfiguration)
        {
        }

        public AppCenterPushNotificationService(ILogger logger, HttpClient httpClient, IAppCenterConfiguration appCenterConfiguration)
        {
            this.logger = logger;
            this.httpClient = httpClient;
            this.appCenterConfiguration = this.EnsureConfig(appCenterConfiguration);
        }

        private IAppCenterConfiguration EnsureConfig(IAppCenterConfiguration config)
        {
            if (string.IsNullOrEmpty(config.ApiToken))
            {
                this.logger.Log(LogLevel.Error, $"Invalid ApiToken");
                throw new ArgumentException($"Use ApiToken provided by AppCenter", nameof(config.ApiToken));
            }

            if (string.IsNullOrEmpty(config.OrganizationName))
            {
                this.logger.Log(LogLevel.Error, $"Invalid OrganizationName");
                throw new ArgumentException($"Use OrganizationName provided by AppCenter", nameof(config.OrganizationName));
            }

            if (config.AppNames == null || config.AppNames.Count == 0)
            {
                this.logger.Log(LogLevel.Error, $"Invalid AppNames");
                throw new ArgumentException($"Use AppNames set-up in AppCenter", nameof(config.AppNames));
            }

            return config;
        }

        public async Task<IEnumerable<AppCenterPushResponse>> SendPushNotificationAsync(AppCenterPushMessage appCenterPushMessage)
        {
            if (appCenterPushMessage == null)
            {
                throw new ArgumentNullException(nameof(appCenterPushMessage));
            }

            this.logger.Log(LogLevel.Debug, "SendPushNotificationAsync");
            var pushResponses = new List<AppCenterPushResponse>();

            //if (!this.appCenterConfiguration.AppNames.TryGetValue(target.TargetDevicePlatform, out var appName, StringComparison.InvariantCultureIgnoreCase))
            //{
            //    this.logger.Log(LogLevel.Warning, "App Center app name not found for " + target.TargetDevicePlatform);
            //}

            var organizationName = this.appCenterConfiguration.OrganizationName;
            var apiToken = this.appCenterConfiguration.ApiToken;
            var appNames = this.appCenterConfiguration.AppNames;

            foreach (var appNameMappings in appNames)
            {
                AppCenterPushResponse appCenterPushResponse;
                try
                {
                    var appName = appNameMappings.Value;
                    var requestUri = $"https://appcenter.ms/api/v0.1/apps/{organizationName}/{appName}/push/notifications";
                    this.logger.Log(LogLevel.Debug, $"SendPushNotificationAsync with requestUri={requestUri}");

                    var serialized = JsonConvert.SerializeObject(appCenterPushMessage);
                    var httpContent = new StringContent(serialized, Encoding.UTF8, "application/json");
                    httpContent.Headers.TryAddWithoutValidation("X-API-Token", apiToken);

                    var httpResponseMessage = await this.httpClient.PostAsync(requestUri, httpContent);

                    var jsonResponse = await httpResponseMessage.Content.ReadAsStringAsync();
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        appCenterPushResponse = JsonConvert.DeserializeObject<AppCenterPushSuccess>(jsonResponse);
                    }
                    else
                    {
                        appCenterPushResponse = JsonConvert.DeserializeObject<AppCenterPushError>(jsonResponse);
                    }
                }
                catch (Exception ex)
                {
                    var errorMessage = $"Failed to send push notification request to app center: {ex.Message}";
                    this.logger.Log(LogLevel.Error, errorMessage);

                    appCenterPushResponse = new AppCenterPushError
                    {
                        ErrorMessage = errorMessage,
                        ErrorCode = "-1"
                    };
                }

                appCenterPushResponse.RuntimePlatform = appNameMappings.Key;
                pushResponses.Add(appCenterPushResponse);
            }

            return pushResponses;
        }
    }
}