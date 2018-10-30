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

            return property;
        }
    }
}