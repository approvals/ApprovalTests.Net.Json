using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ObjectApproval
{
    public static class SerializerBuilder
    {
        static Dictionary<Type, List<string>> ignoredMembers = new Dictionary<Type, List<string>>();
        static List<Type> ignoredTypes = new List<Type>();

        public static void AddIgnore<T>(Expression<Func<T, object>> expression)
        {
            Guard.AgainstNull(expression, nameof(expression));
            if (expression.Body is UnaryExpression unary)
            {
                if (unary.Operand is MemberExpression unaryMember)
                {
                    var declaringType = unaryMember.Member.DeclaringType;
                    var memberName = unaryMember.Member.Name;
                    AddIgnore(declaringType, memberName);
                    return;
                }
            }

            if (expression.Body is MemberExpression member)
            {
                var declaringType = member.Member.DeclaringType;
                var memberName = member.Member.Name;
                AddIgnore(declaringType, memberName);
                return;
            }

            throw new ArgumentException("expression");
        }

        public static void AddIgnore(Type declaringType, string memberName)
        {
            Guard.AgainstNull(declaringType, nameof(declaringType));
            Guard.AgainstNullOrEmpty(memberName, nameof(memberName));
            if (!ignoredMembers.TryGetValue(declaringType, out var list))
            {
                ignoredMembers[declaringType] = list = new List<string>();
            }

            list.Add(memberName);
        }

        public static void AddIgnore<T>()
        {
            ignoredTypes.Add(typeof(T));
        }

        public static bool IgnoreEmptyCollections { get; set; } = true;
        public static bool ScrubGuids { get; set;} = true;
        public static bool ScrubDateTimes { get; set;} = true;
        public static bool QuoteNames { get; set;} = false;
        public static bool UseDoubleQuotes { get; set;} = false;

        public static JsonSerializerSettings BuildSettings(
            bool? ignoreEmptyCollections = true,
            bool? scrubGuids = true,
            bool? scrubDateTimes = true)
        {
            var ignoreEmptyCollectionsVal = ignoreEmptyCollections.GetValueOrDefault(IgnoreEmptyCollections);
            var scrubGuidsVal = scrubGuids.GetValueOrDefault(ScrubGuids);
            var scrubDateTimesVal = scrubDateTimes.GetValueOrDefault(ScrubDateTimes);

            #region defaultSerialization

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            #endregion
            settings.SerializationBinder = new ShortNameBinder();
            settings.ContractResolver = new CustomContractResolver(ignoreEmptyCollectionsVal, ignoredMembers, ignoredTypes);
            AddConverters(scrubGuidsVal, scrubDateTimesVal, settings);
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