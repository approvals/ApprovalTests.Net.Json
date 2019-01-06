using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ObjectApproval
{
    public class CustomContractResolver : DefaultContractResolver
    {
        bool ignoreEmptyCollections;

        public CustomContractResolver(bool ignoreEmptyCollections)
        {
            this.ignoreEmptyCollections = ignoreEmptyCollections;
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
            }

            property.ValueProvider = new CustomValueProvider(property.ValueProvider, property.PropertyType);

            return property;
        }
    }
}