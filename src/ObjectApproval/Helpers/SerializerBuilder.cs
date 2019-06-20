using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// ReSharper disable UseObjectOrCollectionInitializer

namespace ObjectApproval
{
    public static class SerializerBuilder
    {
        private static ISerializerSettingsStore _storage;

        static SerializerBuilder()
        {
            _storage = SerializerSettingsStorageFactory.Create();
            SetupDefaultIgnores();
        }

        public static void Reset()
        {
            _storage.Reset();
            SetupDefaultIgnores();
        }

        private static void SetupDefaultIgnores()
        {
            IgnoreMembersThatThrow<NotImplementedException>();
            IgnoreMembersThatThrow<NotSupportedException>();
            IgnoreMember<Exception>(x => x.HResult);
            IgnoreMember<Exception>(x => x.StackTrace);
        }

        public static bool IgnoreEmptyCollections { get => _storage.IgnoreEmptyCollections; set => _storage.IgnoreEmptyCollections = value; }
        public static bool IgnoreFalse { get => _storage.IgnoreFalse; set => _storage.IgnoreFalse = value; }
        public static bool ScrubGuids { get => _storage.ScrubGuids; set => _storage.ScrubGuids = value; }
        public static bool ScrubDateTimes { get => _storage.ScrubDateTimes; set => _storage.ScrubDateTimes = value; }

        public static Action<JsonSerializerSettings> ExtraSettings { get => _storage.ExtraSettings; set => _storage.ExtraSettings = value; }

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
            var list = _storage.IgnoreMembersByName.GetOrAdd(declaringType, _ => new ConcurrentBag<string>());
            list.Add(name);
        }

        public static void IgnoreInstance<T>(Func<T,bool> shouldIgnore)
        {
            Guard.AgainstNull(shouldIgnore, nameof(shouldIgnore));
            var type = typeof(T);
            IgnoreInstance(
                type,
                target =>
                {
                    var arg = (T)target;
                    return shouldIgnore(arg);
                });
        }

        public static void IgnoreInstance(Type type, Func<object,bool> shouldIgnore)
        {
            Guard.AgainstNull(shouldIgnore, nameof(shouldIgnore));
            var list = _storage.IgnoredInstances.GetOrAdd(type, _ => new ConcurrentBag<Func<object,bool>>());
            list.Add(shouldIgnore);
        }

        public static void IgnoreMembersWithType<T>()
        {
            _storage.IgnoreMembersWithType.Add(typeof(T));
        }

        public static void IgnoreMembersThatThrow<T>()
            where T : Exception
        {
            _storage.IgnoreMembersThatThrow.Add(x => x is T);
        }

        public static void IgnoreMembersThatThrow(Func<Exception, bool> item)
        {
            IgnoreMembersThatThrow<Exception>(item);
        }

        public static void IgnoreMembersThatThrow<T>(Func<T, bool> item)
            where T : Exception
        {
            Guard.AgainstNull(item, nameof(item));
            _storage.IgnoreMembersThatThrow.Add(x =>
            {
                if (x is T exception)
                {
                    return item(exception);
                }

                return false;
            });
        }

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
                _storage.IgnoreMembersByName,
                _storage.IgnoreMembersWithType,
                _storage.IgnoreMembersThatThrow,
                _storage.IgnoredInstances);
            AddConverters(scrubGuidsVal, scrubDateTimesVal, settings);
            ExtraSettings(settings);
            return settings;
        }

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