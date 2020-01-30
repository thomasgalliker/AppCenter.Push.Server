using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AppCenter.Push.Server.Logging;
using AppCenter.Push.Server.Messages;
using AppCenter.Push.Server.Model;
using AppCenter.Push.Server.Tests.Mocks;
using AppCenter.Push.Server.Tests.Testdata;
using FluentAssertions;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace AppCenter.Push.Server.Tests
{
    [Trait("Category", "UnitTests")]
    public class AppCenterPushNotificationServiceTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public AppCenterPushNotificationServiceTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task ShouldSendPushNotificationAsync_AccountIdsTarget_Success()
        {
            // Arrange
            var loggerMock = new Mock<ILogger>();

            var httpClientMock = new HttpClientMock();
            httpClientMock.SetupSendAsync()
                .ReturnsAsync(HttpResponseMessages.Success("notification_id_test"))
                .Verifiable();

            var appCenterConfiguration = new TestAppCenterConfiguration();
            var pushNotificationService = new AppCenterPushNotificationService(loggerMock.Object, httpClientMock.Object, appCenterConfiguration);

            var appCenterPushMessage = new AppCenterPushMessage
            {
                Content = new AppCenterPushContent
                {
                    Name = $"AppCenterPushAccountIdsTarget_{Guid.NewGuid():B}",
                    Title = "Push From App Center",
                    Body = "Hello! Isn't this an amazing notification message?",
                    CustomData = new Dictionary<string, string> { { "key", "value" } }
                },
                Target = new AppCenterPushAccountIdsTarget
                {
                    AccountIds = new List<string>
                    {
                        "A1DF0327-3945-4B24-B22C-CC34367BEFE3",
                        "DF2D5140-CF24-4921-9045-9FE963112981",
                        "7A3E97D4-3BDA-4DFB-89CD-4C46AAEFF548",
                    }
                }
            };

            // Act
            var responseDtos = await pushNotificationService.SendPushNotificationAsync(appCenterPushMessage);

            // Assert
            responseDtos.Should().HaveCount(2);
            responseDtos.Should().AllBeOfType<AppCenterPushSuccess>();
            var appCenterPushResponse = responseDtos.ElementAt(0) as AppCenterPushSuccess;
            appCenterPushResponse.RuntimePlatform.Should().Be(RuntimePlatform.Android);
            appCenterPushResponse.NotificationId.Should().Be("notification_id_test");

            httpClientMock.VerifySendAsync(
                request => request.Method == HttpMethod.Post &&
                           request.RequestUri == new Uri("https://appcenter.ms/api/v0.1/apps/testOrg/TestApp.Android/push/notifications"),
                Times.Exactly(1)
            );
            httpClientMock.VerifySendAsync(
                request => request.Method == HttpMethod.Post &&
                           request.RequestUri == new Uri("https://appcenter.ms/api/v0.1/apps/testOrg/TestApp.iOS/push/notifications"),
                Times.Exactly(1)
            );
        }
        
        [Fact]
        public async Task ShouldSendPushNotificationAsync_AccountIdsTarget_Unauthorized()
        {
            // Arrange
            var loggerMock = new Mock<ILogger>();

            var httpClientMock = new HttpClientMock();
            httpClientMock.SetupSendAsync()
                .ReturnsAsync(HttpResponseMessages.Unauthorized("errorMessage", "errorCode"))
                .Verifiable();

            var appCenterConfiguration = new TestAppCenterConfiguration();
            var pushNotificationService = new AppCenterPushNotificationService(loggerMock.Object, httpClientMock.Object, appCenterConfiguration);

            var appCenterPushMessage = new AppCenterPushMessage
            {
                Content = new AppCenterPushContent
                {
                    Name = $"AppCenterPushAccountIdsTarget_{Guid.NewGuid():B}",
                    Title = "Push From App Center",
                    Body = "Hello! Isn't this an amazing notification message?",
                    CustomData = new Dictionary<string, string> { { "key", "value" } }
                },
                Target = new AppCenterPushAccountIdsTarget
                {
                    AccountIds = new List<string>
                    {
                        "A1DF0327-3945-4B24-B22C-CC34367BEFE3",
                        "DF2D5140-CF24-4921-9045-9FE963112981",
                        "7A3E97D4-3BDA-4DFB-89CD-4C46AAEFF548",
                    }
                }
            };

            // Act
            var responseDtos = await pushNotificationService.SendPushNotificationAsync(appCenterPushMessage);

            // Assert
            responseDtos.Should().HaveCount(2);
            responseDtos.Should().AllBeOfType<AppCenterPushError>();

            var appCenterPushResponse0 = responseDtos.ElementAt(0) as AppCenterPushError;
            appCenterPushResponse0.RuntimePlatform.Should().Be(RuntimePlatform.Android);
            appCenterPushResponse0.ErrorMessage.Should().Be("errorMessage");
            appCenterPushResponse0.ErrorCode.Should().Be("errorCode");

            var appCenterPushResponse1 = responseDtos.ElementAt(1) as AppCenterPushError;
            appCenterPushResponse1.RuntimePlatform.Should().Be(RuntimePlatform.iOS);
            appCenterPushResponse1.ErrorMessage.Should().Be("errorMessage");
            appCenterPushResponse1.ErrorCode.Should().Be("errorCode");

            httpClientMock.VerifySendAsync(
                request => request.Method == HttpMethod.Post &&
                           request.RequestUri == new Uri("https://appcenter.ms/api/v0.1/apps/testOrg/TestApp.Android/push/notifications"),
                Times.Exactly(1)
            );
            httpClientMock.VerifySendAsync(
                request => request.Method == HttpMethod.Post &&
                           request.RequestUri == new Uri("https://appcenter.ms/api/v0.1/apps/testOrg/TestApp.iOS/push/notifications"),
                Times.Exactly(1)
            );
        }
        
        [Fact]
        public async Task ShouldSendPushNotificationAsync_AccountIdsTarget_ThrowsException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger>();

            var httpClientMock = new HttpClientMock();
            httpClientMock.SetupSendAsync()
                .Throws(new Exception($"No such host found"))
                .Verifiable();

            var appCenterConfiguration = new TestAppCenterConfiguration();
            var pushNotificationService = new AppCenterPushNotificationService(loggerMock.Object, httpClientMock.Object, appCenterConfiguration);

            var appCenterPushMessage = new AppCenterPushMessage
            {
                Content = new AppCenterPushContent
                {
                    Name = $"AppCenterPushAccountIdsTarget_{Guid.NewGuid():B}",
                    Title = "Push From App Center",
                    Body = "Hello! Isn't this an amazing notification message?",
                    CustomData = new Dictionary<string, string> { { "key", "value" } }
                },
                Target = new AppCenterPushAccountIdsTarget
                {
                    AccountIds = new List<string>
                    {
                        "A1DF0327-3945-4B24-B22C-CC34367BEFE3",
                        "DF2D5140-CF24-4921-9045-9FE963112981",
                        "7A3E97D4-3BDA-4DFB-89CD-4C46AAEFF548",
                    }
                }
            };

            // Act
            var responseDtos = await pushNotificationService.SendPushNotificationAsync(appCenterPushMessage);

            // Assert
            responseDtos.Should().HaveCount(2);
            responseDtos.Should().AllBeOfType<AppCenterPushError>();

            var appCenterPushResponse0 = responseDtos.ElementAt(0) as AppCenterPushError;
            appCenterPushResponse0.RuntimePlatform.Should().Be(RuntimePlatform.Android);
            appCenterPushResponse0.ErrorMessage.Should().Be("Failed to send push notification request to app center: No such host found");
            appCenterPushResponse0.ErrorCode.Should().Be("-1");

            var appCenterPushResponse1 = responseDtos.ElementAt(1) as AppCenterPushError;
            appCenterPushResponse1.RuntimePlatform.Should().Be(RuntimePlatform.iOS);
            appCenterPushResponse1.ErrorMessage.Should().Be("Failed to send push notification request to app center: No such host found");
            appCenterPushResponse1.ErrorCode.Should().Be("-1");

            httpClientMock.VerifySendAsync(
                request => request.Method == HttpMethod.Post &&
                           request.RequestUri == new Uri("https://appcenter.ms/api/v0.1/apps/testOrg/TestApp.Android/push/notifications"),
                Times.Exactly(1)
            );
            httpClientMock.VerifySendAsync(
                request => request.Method == HttpMethod.Post &&
                           request.RequestUri == new Uri("https://appcenter.ms/api/v0.1/apps/testOrg/TestApp.iOS/push/notifications"),
                Times.Exactly(1)
            );
        }

        [Fact]
        public async Task ShouldSendPushNotificationAsync_DevicesTarget_Success()
        {
            // Arrange
            var loggerMock = new Mock<ILogger>();

            var httpClientMock = new HttpClientMock();
            httpClientMock.SetupSendAsync()
                .ReturnsAsync(HttpResponseMessages.Success("notification_id_test"))
                .Verifiable();

            var appCenterConfiguration = new TestAppCenterConfiguration();
            var pushNotificationService = new AppCenterPushNotificationService(loggerMock.Object, httpClientMock.Object, appCenterConfiguration);

            var appCenterPushMessage = new AppCenterPushMessage
            {
                Content = new AppCenterPushContent
                {
                    Name = $"AppCenterPushDevicesTarget_{Guid.NewGuid():B}",
                    Title = "Push From App Center",
                    Body = "Hello! Isn't this an amazing notification message?",
                    CustomData = new Dictionary<string, string> { { "key", "value" } }
                },
                Target = new AppCenterPushDevicesTarget
                {
                    Devices = new List<string>
                    {
                        "A1DF0327-3945-4B24-B22C-CC34367BEFE3",
                        "DF2D5140-CF24-4921-9045-9FE963112981",
                        "7A3E97D4-3BDA-4DFB-89CD-4C46AAEFF548",
                    }
                }
            };

            // Act
            var responseDtos = await pushNotificationService.SendPushNotificationAsync(appCenterPushMessage);

            // Assert
            responseDtos.Should().HaveCount(2);
            responseDtos.Should().AllBeOfType<AppCenterPushSuccess>();
            var appCenterPushResponse = responseDtos.ElementAt(0) as AppCenterPushSuccess;
            appCenterPushResponse.RuntimePlatform.Should().Be(RuntimePlatform.Android);
            appCenterPushResponse.NotificationId.Should().Be("notification_id_test");

            httpClientMock.VerifySendAsync(
                request => request.Method == HttpMethod.Post &&
                           request.RequestUri == new Uri("https://appcenter.ms/api/v0.1/apps/testOrg/TestApp.Android/push/notifications"),
                Times.Exactly(1)
            );
            httpClientMock.VerifySendAsync(
                request => request.Method == HttpMethod.Post &&
                           request.RequestUri == new Uri("https://appcenter.ms/api/v0.1/apps/testOrg/TestApp.iOS/push/notifications"),
                Times.Exactly(1)
            );
        }

        [Fact]
        public async Task ShouldSendPushNotificationAsync_AudiencesTarget_Success()
        {
            // Arrange
            var loggerMock = new Mock<ILogger>();

            var httpClientMock = new HttpClientMock();
            httpClientMock.SetupSendAsync()
                .ReturnsAsync(HttpResponseMessages.Success("notification_id_test"))
                .Verifiable();

            var appCenterConfiguration = new TestAppCenterConfiguration();
            var pushNotificationService = new AppCenterPushNotificationService(loggerMock.Object, httpClientMock.Object, appCenterConfiguration);

            var appCenterPushMessage = new AppCenterPushMessage
            {
                Content = new AppCenterPushContent
                {
                    Name = $"AppCenterPushAudiencesTarget_{Guid.NewGuid():B}",
                    Title = "Push From App Center",
                    Body = "Hello! Isn't this an amazing notification message?",
                    CustomData = new Dictionary<string, string> { { "key", "value" } }
                },
                Target = new AppCenterPushAudiencesTarget
                {
                    Audiences = new List<string>
                    {
                        "test_audience_1",
                        "test_audience_2",
                        "test_audience_3",
                    }
                }
            };

            // Act
            var responseDtos = await pushNotificationService.SendPushNotificationAsync(appCenterPushMessage);

            // Assert
            responseDtos.Should().HaveCount(2);
            responseDtos.Should().AllBeOfType<AppCenterPushSuccess>();
            var appCenterPushResponse = responseDtos.ElementAt(0) as AppCenterPushSuccess;
            appCenterPushResponse.RuntimePlatform.Should().Be(RuntimePlatform.Android);
            appCenterPushResponse.NotificationId.Should().Be("notification_id_test");

            httpClientMock.VerifySendAsync(
                request => request.Method == HttpMethod.Post &&
                           request.RequestUri == new Uri("https://appcenter.ms/api/v0.1/apps/testOrg/TestApp.Android/push/notifications"),
                Times.Exactly(1)
            );
            httpClientMock.VerifySendAsync(
                request => request.Method == HttpMethod.Post &&
                           request.RequestUri == new Uri("https://appcenter.ms/api/v0.1/apps/testOrg/TestApp.iOS/push/notifications"),
                Times.Exactly(1)
            );
        }

        [Fact]
        public async Task ShouldSendPushNotificationAsync_UserIdsTarget_Success()
        {
            // Arrange
            var loggerMock = new Mock<ILogger>();

            var httpClientMock = new HttpClientMock();
            httpClientMock.SetupSendAsync()
                .ReturnsAsync(HttpResponseMessages.Success("notification_id_test"))
                .Verifiable();

            var appCenterConfiguration = new TestAppCenterConfiguration();
            var pushNotificationService = new AppCenterPushNotificationService(loggerMock.Object, httpClientMock.Object, appCenterConfiguration);

            var appCenterPushMessage = new AppCenterPushMessage
            {
                Content = new AppCenterPushContent
                {
                    Name = $"AppCenterPushUserIdsTarget_{Guid.NewGuid():B}",
                    Title = "Push From App Center",
                    Body = "Hello! Isn't this an amazing notification message?",
                    CustomData = new Dictionary<string, string> { { "key", "value" } }
                },
                Target = new AppCenterPushUserIdsTarget
                {
                    UserIds = new List<string>
                    {
                        "a0061b36-8a50-4e2e-aaab-1c5849dccf30",
                        "91145edf-53ce-4f74-8ebd-8934f08905f6",
                        "7ee7354a-f69a-4a79-929a-b50b66a05518",
                    }
                }
            };

            // Act
            var responseDtos = await pushNotificationService.SendPushNotificationAsync(appCenterPushMessage);

            // Assert
            responseDtos.Should().HaveCount(2);
            responseDtos.Should().AllBeOfType<AppCenterPushSuccess>();
            var appCenterPushResponse = responseDtos.ElementAt(0) as AppCenterPushSuccess;
            appCenterPushResponse.RuntimePlatform.Should().Be(RuntimePlatform.Android);
            appCenterPushResponse.NotificationId.Should().Be("notification_id_test");

            httpClientMock.VerifySendAsync(
                request => request.Method == HttpMethod.Post &&
                           request.RequestUri == new Uri("https://appcenter.ms/api/v0.1/apps/testOrg/TestApp.Android/push/notifications"),
                Times.Exactly(1)
            );
            httpClientMock.VerifySendAsync(
                request => request.Method == HttpMethod.Post &&
                           request.RequestUri == new Uri("https://appcenter.ms/api/v0.1/apps/testOrg/TestApp.iOS/push/notifications"),
                Times.Exactly(1)
            );
        }
    }
}