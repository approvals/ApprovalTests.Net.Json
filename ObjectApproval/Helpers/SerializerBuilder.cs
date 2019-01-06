using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ObjectApproval
{
    public static class SerializerBuilder
    {
        static Dictionary<Type,List<string>> ignored = new Dictionary<Type, List<string>>();

        public static void AddIgnore<T>(Expression<Func<T, object>> expression)
        {
            if (!(expression.Body is MemberExpression member))
            {
                throw new ArgumentException("expression");
            }
            if (!ignored.TryGetValue(member.Member.DeclaringType, out var list))
            {
                ignored[member.Member.DeclaringType]=list = new List<string>();
            }
            list.Add(member.Member.Name);
        }

        public static JsonSerializerSettings BuildSettings(
            bool ignoreEmptyCollections = true,
            bool scrubGuids = true,
            bool scrubDateTimes = true)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                SerializationBinder = new ShortNameBinder(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ContractResolver = new CustomContractResolver(ignoreEmptyCollections, ignored)
            };
            AddConverters(scrubGuids, scrubDateTimes, settings);
            ExtraSettings(settings);
            return settings;
        }

        public static Action<JsonSerializerSettings> ExtraSettings = settings => {};

        static void AddConverters(bool scrubGuids, bool scrubDateTimes, JsonSerializerSettings settings)
        {
            var converters = settings.Converters;
            converters.Add(new StringEnumConverter());
            if (scrubGuids && scrubDateTimes)
            {
                var guidScrubbingConverter = new Scrubber<Guid>();
                converters.Add(guidScrubbingConverter);
                var dateTimeScrubber = new Scrubber<DateTime>();
                converters.Add(dateTimeScrubber);
                var dateTimeOffsetScrubber = new Scrubber<DateTimeOffset>();
                converters.Add(dateTimeOffsetScrubber);
                converters.Add(new StringScrubber(guidScrubbingConverter, dateTimeScrubber, dateTimeOffsetScrubber));
                return;
            }

            if (scrubGuids)
            {
                var guidScrubbingConverter = new Scrubber<Guid>();
                converters.Add(guidScrubbingConverter);
                converters.Add(new StringScrubber(guidScrubbingConverter, null, null));
            }

            if (scrubDateTimes)
            {
                var dateTimeScrubber = new Scrubber<DateTime>();
                converters.Add(dateTimeScrubber);
                var dateTimeOffsetScrubber = new Scrubber<DateTimeOffset>();
                converters.Add(dateTimeOffsetScrubber);
                converters.Add(new StringScrubber(null, dateTimeScrubber, dateTimeOffsetScrubber));
            }
        }
    }
}