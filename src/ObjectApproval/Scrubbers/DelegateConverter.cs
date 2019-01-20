using System;
using Newtonsoft.Json;

namespace ObjectApproval
{
    public class DelegateConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is null)
            {
                return;
            }
            var multicastDelegate = (MulticastDelegate)value;

            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            writer.WriteValue(TypeNameConverter.GetName(multicastDelegate.GetType()));
            writer.WritePropertyName("Target");
            writer.WriteValue(TypeNameConverter.GetName(multicastDelegate.Method.DeclaringType));
            writer.WritePropertyName("Method");
            writer.WriteValue(multicastDelegate.Method.ToString());
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(MulticastDelegate).IsAssignableFrom(objectType);
        }
    }
}