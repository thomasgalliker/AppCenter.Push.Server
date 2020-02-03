using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AppCenter.Push.Server.Console.Internals;
using AppCenter.Push.Server.Messages;
using AppCenter.Push.Server.Model;
using Microsoft.Extensions.Configuration;

namespace AppCenter.Push.Server.Console
{
    using Console = System.Console;

    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Console.WriteLine($"AppCenter.Push.Server.Console [Version 1.0.0.0]");
            Console.WriteLine($"(c) 2020 superdev gmbh. All rights reserved.");

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
                Console.WriteLine("");
                Console.WriteLine($"API Token:");
                var apiToken = Console.ReadLine();
                appCenterConfiguration.ApiToken = apiToken;

                Console.WriteLine("");
                Console.WriteLine($"Organization Name:");
                var organizationName = Console.ReadLine();
                appCenterConfiguration.OrganizationName = organizationName;

                Console.WriteLine("");
                Console.WriteLine($"App Name [iOS]:");
                var appNameiOS = Console.ReadLine();

                Console.WriteLine("");
                Console.WriteLine($"App Name [Android]:");
                var appNameAndroid = Console.ReadLine();

                Console.WriteLine("");
                Console.WriteLine($"App Name [UWP]:");
                var appNameUWP = Console.ReadLine();

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

            Console.WriteLine("");
            Console.WriteLine($"Enter UserIds:");
            var userIdsInput = Console.ReadLine();
            var userIds = userIdsInput?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

            Console.WriteLine("");
            Console.WriteLine("Press [Enter] to send push notifications: ");
            Console.ReadLine();
            Console.Clear();

            try
            {
                SendPushNotificationAsync(appCenterConfiguration, userIds).Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadKey();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine($"{e.ExceptionObject}");
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
                Console.WriteLine("");
                Console.WriteLine("AppCenter responses:");
                Console.WriteLine("====================");
                foreach (var appCenterPushResponse in appCenterPushResponses)
                {
                    if (appCenterPushResponse is AppCenterPushSuccess success)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"--> {appCenterConfiguration.AppNames[success.RuntimePlatform]}: {success.NotificationId}");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    else if (appCenterPushResponse is AppCenterPushError error)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"--> {appCenterConfiguration.AppNames[error.RuntimePlatform]}: Code={error.ErrorCode}, Message={error.ErrorMessage}");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                }
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("No responses from AppCenter.");
            }
        
            Console.ReadKey();
        }
    }
}
