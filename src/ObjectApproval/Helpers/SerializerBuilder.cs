using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
// ReSharper disable UseObjectOrCollectionInitializer

namespace ObjectApproval
{
    public static class SerializerBuilder
    {
        static SerializerBuilder()
        {
            IgnoreMembersThatThrow<NotImplementedException>();
            IgnoreMembersThatThrow<NotSupportedException>();
            IgnoreMember<Exception>(x => x.HResult);
            IgnoreMember<Exception>(x => x.StackTrace);
        }

        static Dictionary<Type, List<string>> ignoreMembersByName = new Dictionary<Type, List<string>>();

        public static void IgnoreMember<T>(Expression<Func<T, object>> expression)
        {
            Guard.AgainstNull(expression, nameof(expression));
            if (expression.Body is UnaryExpression unary)
            {
                if (unary.Operand is MemberExpression unaryMember)
                {
                    var declaringType = unaryMember.Member.DeclaringType;
                    var memberName = unaryMember.Member.Name;
                    IgnoreMember(declaringType, memberName);
                    return;
                }
            }

            if (expression.Body is MemberExpression member)
            {
                var declaringType = member.Member.DeclaringType;
                var memberName = member.Member.Name;
                IgnoreMember(declaringType, memberName);
                return;
            }

            throw new ArgumentException("expression");
        }

        public static void IgnoreMember(Type declaringType, string name)
        {
            Guard.AgainstNull(declaringType, nameof(declaringType));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            if (!ignoreMembersByName.TryGetValue(declaringType, out var list))
            {
                ignoreMembersByName[declaringType] = list = new List<string>();
            }

            list.Add(name);
        }

        static List<Type> ignoreMembersWithType = new List<Type>();

        public static void IgnoreMembersWithType<T>()
        {
            ignoreMembersWithType.Add(typeof(T));
        }

        static List<Func<Exception, bool>> ignoreMembersThatThrow = new List<Func<Exception, bool>>();

        public static void IgnoreMembersThatThrow<T>()
            where T : Exception
        {
            ignoreMembersThatThrow.Add(x => x is T);
        }

        public static void IgnoreMembersThatThrow(Func<Exception, bool> item)
        {
            IgnoreMembersThatThrow<Exception>(item);
        }

        public static void IgnoreMembersThatThrow<T>(Func<T, bool> item)
            where T : Exception
        {
            Guard.AgainstNull(item, nameof(item));
            ignoreMembersThatThrow.Add(x =>
            {
                if (x is T exception)
                {
                    return item(exception);
                }

                return false;
            });
        }

        public static bool IgnoreEmptyCollections { get; set; } = true;
        public static bool IgnoreFalse { get; set; } = true;
        public static bool ScrubGuids { get; set; } = true;
        public static bool ScrubDateTimes { get; set; } = true;
        public static bool QuoteNames { get; set; } = false;
        public static bool UseDoubleQuotes { get; set; } = false;

        public static JsonSerializerSettings BuildSettings(
            bool? ignoreEmptyCollections = null,
            bool? scrubGuids = null,
            bool? scrubDateTimes = null,
            bool? ignoreFalse = null)
        {
            var ignoreEmptyCollectionsVal = ignoreEmptyCollections.GetValueOrDefault(IgnoreEmptyCollections);
            var ignoreFalseVal = ignoreFalse.GetValueOrDefault(IgnoreFalse);
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
            settings.ContractResolver = new CustomContractResolver(
                ignoreEmptyCollectionsVal,
                ignoreFalseVal,
                ignoreMembersByName,
                ignoreMembersWithType,
                ignoreMembersThatThrow);
            AddConverters(scrubGuidsVal, scrubDateTimesVal, settings);
            ExtraSettings(settings);
            return settings;
        }

        public static Action<JsonSerializerSettings> ExtraSettings = settings => { };

        static void AddConverters(bool scrubGuids, bool scrubDateTimes, JsonSerializerSettings settings)
        {
            var converters = settings.Converters;
            converters.Add(new StringEnumConverter());
            converters.Add(new DelegateConverter());
            converters.Add(new TypeConverter());
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