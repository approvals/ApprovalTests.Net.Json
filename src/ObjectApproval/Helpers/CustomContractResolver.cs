using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ObjectApproval
{
    public class CustomContractResolver : DefaultContractResolver
    {
        bool ignoreEmptyCollections;
        IReadOnlyDictionary<Type, List<string>> ignored;
        IReadOnlyList<Type> ignoredTypes;

        public CustomContractResolver(bool ignoreEmptyCollections) :
            this(ignoreEmptyCollections, new Dictionary<Type, List<string>>(), new List<Type>())
        {
        }

        public CustomContractResolver(bool ignoreEmptyCollections, IReadOnlyDictionary<Type, List<string>> ignored, IReadOnlyList<Type> ignoredTypes)
        {
            Guard.AgainstNull(ignored, nameof(ignored));
            Guard.AgainstNull(ignoredTypes, nameof(ignoredTypes));
            this.ignoreEmptyCollections = ignoreEmptyCollections;
            this.ignored = ignored;
            this.ignoredTypes = ignoredTypes;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (ignoreEmptyCollections)
            {
                property.SkipEmptyCollections(member);
            }


            if (member.GetCustomAttribute<ObsoleteAttribute>(true) != null)
            {
                property.Ignored = true;
                return property;
            }

            if (ignoredTypes.Any(x => x.IsAssignableFrom(property.PropertyType)))
            {
                property.Ignored = true;
                return property;
            }

            foreach (var keyValuePair in ignored)
            {
                if (keyValuePair.Value.Contains(property.PropertyName))
                {
                    if (keyValuePair.Key.IsAssignableFrom(property.DeclaringType))
                    {
                        property.Ignored = true;
                        return property;
                    }
                }
            }

            property.ValueProvider = new CustomValueProvider(property.ValueProvider, property.PropertyType);

            return property;
        }
    }
}