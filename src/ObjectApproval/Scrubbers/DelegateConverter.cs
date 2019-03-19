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

            var @delegate = (Delegate)value;

            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            writer.WriteValue(TypeNameConverter.GetName(@delegate.GetType()));
            writer.WritePropertyName("Target");
            writer.WriteValue(TypeNameConverter.GetName(@delegate.Method.DeclaringType));
            writer.WritePropertyName("Method");
            var s = @delegate.Method.ToString();
            writer.WriteValue(CleanMethodName(s));
            writer.WriteEndObject();
        }

        public static string CleanMethodName(string s)
        {
            return s;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Delegate).IsAssignableFrom(objectType);
        }
    }
}