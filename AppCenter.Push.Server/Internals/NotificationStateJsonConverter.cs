using System;
using AppCenter.Push.Server.Messages;
using Newtonsoft.Json;

namespace AppCenter.Push.Server
{
    public class NotificationStateJsonConverter : JsonConverter<NotificationState>
    {
        public override void WriteJson(JsonWriter writer, NotificationState value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override NotificationState ReadJson(JsonReader reader, Type objectType, NotificationState existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}