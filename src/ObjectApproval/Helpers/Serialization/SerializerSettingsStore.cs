using System;
using Newtonsoft.Json;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace ObjectApproval
{
    internal static class SerializerSettingsStorageFactory
    {
        public static ISerializerSettingsStore Create()
        {
            var mode = SerializerThreadingMode.SingleThreaded;

            var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(ObjectApprovalBehaviorAttribute), false);
            if (attributes.Length != 0 && attributes[0] is ObjectApprovalBehaviorAttribute objectApprovalAttribute)
            {
                mode = objectApprovalAttribute.SerializerThreadingMode;
            }

            switch(mode)
            {
                case SerializerThreadingMode.SingleThreaded:
                    return new SingleThreadedSerializerSettingsStore();
                case SerializerThreadingMode.MultiThreaded:
                    return new MultiThreadedSerializerSettingsStore();
            }

            throw new ArgumentException($"Received an unsupported threading mode: {mode.ToString()}");
        }
    }

    internal interface ISerializerSettingsStore
    {
        void Reset();

        ConcurrentDictionary<Type, ConcurrentBag<string>> IgnoreMembersByName { get; }
        ConcurrentDictionary<Type, ConcurrentBag<Func<object, bool>>> IgnoredInstances { get; }
        List<Type> IgnoreMembersWithType { get; }
        List<Func<Exception, bool>> IgnoreMembersThatThrow { get; }

        bool IgnoreEmptyCollections { get; set; }
        bool IgnoreFalse { get; set; }
        bool ScrubGuids { get; set; }
        bool ScrubDateTimes { get; set; }

        Action<JsonSerializerSettings> ExtraSettings { get; set; }
    }

    internal class SingleThreadedSerializerSettingsStore : ISerializerSettingsStore
    {
        public SingleThreadedSerializerSettingsStore()
        {
            Reset();
        }

        public void Reset()
        {
            IgnoreMembersByName.Clear();
            IgnoredInstances.Clear();
            IgnoreMembersWithType.Clear();
            IgnoreMembersThatThrow.Clear();

            IgnoreEmptyCollections = true;
            IgnoreFalse = true;
            ScrubGuids = true;
            ScrubDateTimes = true;

            ExtraSettings = settings => { };
        }

        public ConcurrentDictionary<Type, ConcurrentBag<string>> IgnoreMembersByName { get; } = new ConcurrentDictionary<Type, ConcurrentBag<string>>();
        public ConcurrentDictionary<Type, ConcurrentBag<Func<object, bool>>> IgnoredInstances { get; } = new ConcurrentDictionary<Type, ConcurrentBag<Func<object, bool>>>();
        public List<Type> IgnoreMembersWithType { get; } = new List<Type>();
        public List<Func<Exception, bool>> IgnoreMembersThatThrow { get; } = new List<Func<Exception, bool>>();

        public bool IgnoreEmptyCollections { get; set; }
        public bool IgnoreFalse { get; set; }
        public bool ScrubGuids { get; set; }
        public bool ScrubDateTimes { get; set; }
        public Action<JsonSerializerSettings> ExtraSettings { get; set; }
    }

    internal class MultiThreadedSerializerSettingsStore : ISerializerSettingsStore
    {
        private ThreadLocal<SingleThreadedSerializerSettingsStore> _innerStore = new ThreadLocal<SingleThreadedSerializerSettingsStore>(() => new SingleThreadedSerializerSettingsStore());

        public void Reset()
        {
            _innerStore.Value.Reset();
        }

        public ConcurrentDictionary<Type, ConcurrentBag<string>> IgnoreMembersByName => _innerStore.Value.IgnoreMembersByName;
        public ConcurrentDictionary<Type, ConcurrentBag<Func<object, bool>>> IgnoredInstances => _innerStore.Value.IgnoredInstances;
        public List<Type> IgnoreMembersWithType => _innerStore.Value.IgnoreMembersWithType;
        public List<Func<Exception, bool>> IgnoreMembersThatThrow => _innerStore.Value.IgnoreMembersThatThrow;

        public bool IgnoreEmptyCollections { get => _innerStore.Value.IgnoreEmptyCollections; set => _innerStore.Value.IgnoreEmptyCollections = value; }
        public bool IgnoreFalse { get => _innerStore.Value.IgnoreFalse; set => _innerStore.Value.IgnoreFalse = value; }
        public bool ScrubGuids { get => _innerStore.Value.ScrubGuids; set => _innerStore.Value.ScrubGuids = value; }
        public bool ScrubDateTimes { get => _innerStore.Value.ScrubDateTimes; set => _innerStore.Value.ScrubDateTimes = value; }
        public Action<JsonSerializerSettings> ExtraSettings { get => _innerStore.Value.ExtraSettings; set => _innerStore.Value.ExtraSettings = value; }
    }
}
