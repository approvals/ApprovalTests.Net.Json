using System;
using System.IO;
using System.Text;
using ApprovalTests;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ObjectApproval
{
    public static class ObjectApprover
    {
        public static JsonSerializer JsonSerializer { get; set; }

        static ObjectApprover()
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };
            settings.Converters.Add(new StringEnumConverter());
            JsonSerializer = JsonSerializer.Create(settings);
        }

        public static void VerifyWithJson(object target, Func<string, string> scrubber = null, JsonSerializerSettings jsonSerializerSettings = null)
        {
            var formatJson = AsFormattedJson(target, jsonSerializerSettings);
            if (scrubber == null)
            {
                scrubber = s => s;
            }

            Approvals.Verify(formatJson, scrubber);
        }

        public static string AsFormattedJson(object target, JsonSerializerSettings jsonSerializerSettings = null)
        {
            var builder = new StringBuilder();
            using (var stringWriter = new StringWriter(builder))
            {
                using (var jsonWriter = new JsonTextWriter(stringWriter))
                {
                    var jsonSerializer = GetJsonSerializer(jsonSerializerSettings);

                    jsonWriter.Formatting = jsonSerializer.Formatting;
                    jsonSerializer.Serialize(jsonWriter, target);
                }

                return stringWriter.ToString();
            }
        }

        static JsonSerializer GetJsonSerializer(JsonSerializerSettings jsonSerializerSettings)
        {
            if (jsonSerializerSettings == null)
            {
                return JsonSerializer;
            }

            return JsonSerializer.Create(jsonSerializerSettings);
        }
    }
}