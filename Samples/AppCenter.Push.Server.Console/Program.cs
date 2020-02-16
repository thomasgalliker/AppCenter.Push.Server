using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AppCenter.Push.Server.Console.Internals;
using AppCenter.Push.Server.Messages;
using Microsoft.Extensions.Configuration;

namespace AppCenter.Push.Server.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            System.Console.WriteLine($"AppCenter.Push.Server.Console [Version 1.0.0.0]");
            System.Console.WriteLine($"(c) 2020 superdev gmbh. All rights reserved.");

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var appCenterConfiguration = new ConsoleAppCenterConfiguration();

            var configurationSection = config.GetSection("PushNotification");
            if (configurationSection.Exists())
            {
                // Configuration from json appsettings file
                appCenterConfiguration.ApiToken = configurationSection["ApiToken"];
                appCenterConfiguration.OrganizationName = configurationSection["OrganizationName"];

                var appNamesSections = configurationSection.GetSection("AppNames");
                if (appNamesSections.Exists())
                {
                    foreach (var section in appNamesSections.GetChildren())
                    {
                        appCenterConfiguration.AppNames.Add(new RuntimePlatform(section.Key), section.Value);
                    }
                }
            }
            else
            {
                // Manual configuration
                System.Console.WriteLine("");
                System.Console.WriteLine($"API Token:");
                var apiToken = System.Console.ReadLine();
                appCenterConfiguration.ApiToken = apiToken;

                System.Console.WriteLine("");
                System.Console.WriteLine($"Organization Name:");
                var organizationName = System.Console.ReadLine();
                appCenterConfiguration.OrganizationName = organizationName;

                System.Console.WriteLine("");
                System.Console.WriteLine($"App Name [iOS]:");
                var appNameiOS = System.Console.ReadLine();

                System.Console.WriteLine("");
                System.Console.WriteLine($"App Name [Android]:");
                var appNameAndroid = System.Console.ReadLine();

                System.Console.WriteLine("");
                System.Console.WriteLine($"App Name [UWP]:");
                var appNameUWP = System.Console.ReadLine();

                if (!string.IsNullOrEmpty(appNameiOS))
                {
                    appCenterConfiguration.AppNames.Add(new KeyValuePair<RuntimePlatform, string>(RuntimePlatform.iOS, appNameiOS));
                }

                if (!string.IsNullOrEmpty(appNameAndroid))
                {
                    appCenterConfiguration.AppNames.Add(new KeyValuePair<RuntimePlatform, string>(RuntimePlatform.Android, appNameAndroid));
                }

                if (!string.IsNullOrEmpty(appNameUWP))
                {
                    appCenterConfiguration.AppNames.Add(new KeyValuePair<RuntimePlatform, string>(RuntimePlatform.UWP, appNameUWP));
                }
            }

            System.Console.WriteLine("");
            System.Console.WriteLine($"Enter UserIds:");
            var userIdsInput = System.Console.ReadLine();
            var userIds = userIdsInput?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

            System.Console.WriteLine("");
            System.Console.WriteLine("Press [Enter] to send push notifications: ");
            System.Console.ReadLine();
            System.Console.Clear();

            try
            {
                SendPushNotificationAsync(appCenterConfiguration, userIds).Wait();
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }

            System.Console.ReadKey();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.Console.WriteLine($"{e.ExceptionObject}");
        }

        private static async Task SendPushNotificationAsync(IAppCenterConfiguration appCenterConfiguration, List<string> userIds)
        {
            var dateTimeNow = DateTime.Now;

            var appCenterPushNotificationService = new AppCenterPushNotificationService(new ConsoleLogger(), new HttpClient(), appCenterConfiguration);

            var pushMessage = new AppCenterPushMessage
            {
                Content = new AppCenterPushContent
                {
                    Name = $"AppCenterPushUserIdsTarget_{dateTimeNow:yyyyMMdd_HHmmss}",
                    Title = "AppCenterPushUserIdsTarget",
                    Body = $"This message has been sent at {dateTimeNow:G}",
                    CustomData = new Dictionary<string, string> { { "key", "value" } }
                },
                Target = new AppCenterPushUserIdsTarget
                {
                    UserIds = userIds
                }
            };

            var appCenterPushResponses = await appCenterPushNotificationService.SendPushNotificationAsync(pushMessage);
            if (appCenterPushResponses.Any())
            {
                System.Console.WriteLine("");
                System.Console.WriteLine("SendPushNotificationAsync responses:");
                System.Console.WriteLine("------------------------------------");
                foreach (var appCenterPushResponse in appCenterPushResponses)
                {
                    if (appCenterPushResponse is AppCenterPushSuccess success)
                    {
                        System.Console.ForegroundColor = ConsoleColor.Green;
                        System.Console.WriteLine($"--> {appCenterConfiguration.AppNames[success.RuntimePlatform]}: {success.NotificationId}");
                        System.Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    else if (appCenterPushResponse is AppCenterPushError error)
                    {
                        System.Console.ForegroundColor = ConsoleColor.Red;
                        System.Console.WriteLine($"--> {appCenterConfiguration.AppNames[error.RuntimePlatform]}: Code={error.ErrorCode}, Message={error.ErrorMessage}");
                        System.Console.ForegroundColor = ConsoleColor.Gray;
                    }
                }
            }
            else
            {
                System.Console.WriteLine("");
                System.Console.WriteLine("No responses from AppCenter.");
            }

            System.Console.WriteLine("");
            System.Console.WriteLine("GetPushNotificationsAsync:");
            System.Console.WriteLine("--------------------------");
            System.Console.WriteLine("");
            var notificationOverviewResults = await appCenterPushNotificationService.GetPushNotificationsAsync();
            if (notificationOverviewResults.Any())
            {
                System.Console.WriteLine($"NotificationId\t\t\t\t\t| State\t\t| Success\t| Failure");
                foreach (var result in notificationOverviewResults)
                {
                    System.Console.WriteLine($"{result.NotificationId}\t| {result.State}\t| {result.PnsSendSuccess}\t\t| {result.PnsSendFailure}");
                }
            }
            else
            {
                System.Console.WriteLine("");
                System.Console.WriteLine("No responses from AppCenter.");
            }

            System.Console.ReadKey();
        }
    }
}