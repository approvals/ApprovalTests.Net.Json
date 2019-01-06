using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ObjectApproval
{
    public static class SerializerBuilder
    {
        static Dictionary<Type,List<string>> ignoredMembers = new Dictionary<Type, List<string>>();
        static List<Type> ignoredTypes = new List<Type>();

        public static void AddIgnore<T>(Expression<Func<T, object>> expression)
        {
            if (!(expression.Body is MemberExpression member))
            {
                throw new ArgumentException("expression");
            }
            if (!ignoredMembers.TryGetValue(member.Member.DeclaringType, out var list))
            {
                ignoredMembers[member.Member.DeclaringType]=list = new List<string>();
            }
            list.Add(member.Member.Name);
        }

        public static void AddIgnore<T>()
        {
            ignoredTypes.Add(typeof(T));
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
                ContractResolver = new CustomContractResolver(ignoreEmptyCollections, ignoredMembers,ignoredTypes)
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