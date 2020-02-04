using System;
using System.Reflection;
using AppCenter.Push.Server.Messages;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace AppCenter.Push.Server.Tests.Internals
{
    [Trait("Category", "UnitTests")]
    public class NotificationOverviewResultInternalTests
    {
        [Fact]
        public void ShouldDeserializeNotificationOverviewResultInternal()
        {
            // Arrange
            var json = ResourceLoader.Current.GetEmbeddedResourceString(this.GetType().Assembly, "NotificationOverviewList_Example1.json");

            // Act
            var notificationOverviewResultInternal = JsonConvert.DeserializeObject<NotificationOverviewResultInternal>(json);

            // Assert
            notificationOverviewResultInternal.Should().NotBeNull();
            notificationOverviewResultInternal.Values.Should().HaveCount(30);
        }
    }
}