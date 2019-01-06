using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ObjectApproval
{
    public class CustomContractResolver : DefaultContractResolver
    {
        bool ignoreEmptyCollections;
        IReadOnlyDictionary<Type, List<string>> ignored;

        public CustomContractResolver(bool ignoreEmptyCollections, IReadOnlyDictionary<Type, List<string>> ignored)
        {
            this.ignoreEmptyCollections = ignoreEmptyCollections;
            this.ignored = ignored;
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