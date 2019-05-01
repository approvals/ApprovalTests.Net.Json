using System;
using System.IO;
using System.Text;
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

        public static void VerifyWithJson(object target, Func<string, string> scrubber = null)
        {
            VerifyWithJson(target, scrubber, null);
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
            bool ignoreFalse = true,
            Func<string, string> scrubber = null)
        {
            var settings = SerializerBuilder.BuildSettings(ignoreEmptyCollections, scrubGuids, scrubDateTimes, ignoreFalse);
            var formatJson = AsFormattedJson(target, settings);
            if (scrubber == null)
            {
                scrubber = s => s;
            }

            Approvals.Verify(formatJson, scrubber);
        }

        public static string AsFormattedJson(object target, JsonSerializerSettings jsonSerializerSettings = null)
        {
            var serializer = GetJsonSerializer(jsonSerializerSettings);
            var builder = new StringBuilder();
            using (var stringWriter = new StringWriter(builder))
            using (var writer = new JsonTextWriter(stringWriter))
            {
                writer.QuoteChar = '\'';
                writer.QuoteName = false;
                serializer.Serialize(writer, target);
            }

            builder.Replace(@"\\", @"\");
            return builder.ToString();
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