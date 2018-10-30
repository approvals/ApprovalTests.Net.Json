using System;
using ApprovalTests;
using Newtonsoft.Json;

namespace ObjectApproval
{
    public static class ObjectApprover
    {
        public static void VerifyWithJson(object target)
        {
            VerifyWithJson(target, null);
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

        public static void VerifyWithJson(
            object target,
            bool ignoreEmptyCollections = true,
            bool scrubGuids = true,
            bool scrubDateTimes = true,
            Func<string, string> scrubber = null)
        {
            var settings = SerializerBuilder.BuildSettings(ignoreEmptyCollections, scrubGuids, scrubDateTimes);
            var formatJson = AsFormattedJson(target, settings);
            if (scrubber == null)
            {
                scrubber = s => s;
            }

            Approvals.Verify(formatJson, scrubber);
        }

        public static string AsFormattedJson(object target, JsonSerializerSettings jsonSerializerSettings = null)
        {
            var jsonSerializer = GetJsonSerializer(jsonSerializerSettings);
            return jsonSerializer.Serialize(target);
        }

        static JsonSerializer GetJsonSerializer(JsonSerializerSettings jsonSerializerSettings)
        {
            if (jsonSerializerSettings == null)
            {
                return JsonSerializer.Create(SerializerBuilder.BuildSettings());
            }

            return JsonSerializer.Create(jsonSerializerSettings);
        }
    }
}