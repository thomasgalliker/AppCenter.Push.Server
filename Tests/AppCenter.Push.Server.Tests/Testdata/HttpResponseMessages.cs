using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using AppCenter.Push.Server.Messages;
using Newtonsoft.Json;

namespace AppCenter.Push.Server.Tests.Testdata
{
    public static class HttpResponseMessages
    {
        public static class AppCenterPushResponses
        {
            public static HttpResponseMessage Success(string notificationId)
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(new AppCenterPushSuccess
                    {
                        NotificationId = notificationId
                    }))
                };
            }

            public static HttpResponseMessage Unauthorized(string errorMessage, string errorCode)
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Content = new StringContent(JsonConvert.SerializeObject(new AppCenterPushError
                    {
                        ErrorMessage = errorMessage,
                        ErrorCode = errorCode,
                    }))
                };
            }
        }

        public static class NotificationOverviewResults
        {
            public static HttpResponseMessage Success(string notificationId, int count)
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(new NotificationOverviewResultInternal
                    {
                        Values = GenerateNotificationOverviewResults(notificationId, count).ToList()
                    }))
                };
            }

            private static IEnumerable<NotificationOverviewResult> GenerateNotificationOverviewResults(string notificationId, int count)
            {
                for (int i = 0; i < count; i++)
                {
                    yield return new NotificationOverviewResult
                    {
                        NotificationId = $"{notificationId}_{i}",
                        Name = $"name_{i}",
                        PnsSendFailure = 1,
                        PnsSendSuccess = 2,
                        SendTime = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    };
                }
            }
        }
    }
}