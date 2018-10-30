using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace ObjectApproval
{
    public static class ContractResolutionHelpers
    {
        static bool IsCollection(this Type type)
        {
            return type.GetInterfaces()
                .Any(x => x.IsGenericType &&
                          x.GetGenericTypeDefinition() == typeof(ICollection<>));
        }

        public static void SkipEmptyCollections(this JsonProperty property, MemberInfo member)
        {
            if (property.PropertyType == typeof(string))
            {
                return;
            }

            if (!property.PropertyType.IsCollection())
            {
                return;
            }

            property.ShouldSerialize = instance =>
            {
                var collection = GetCollection(member, instance);

                if (collection == null)
                {
                    // if the list is null, we defer the decision to NullValueHandling
                    return true;
                }

                // check to see if there is at least one item in the Enumerable
                return collection.GetEnumerator().MoveNext();
            };
        }

        static IEnumerable GetCollection(MemberInfo member, object instance)
        {
            // this value could be in a public field or public property
            if (member is PropertyInfo propertyInfo)
            {
                return (IEnumerable)propertyInfo.GetValue(instance, null);
            }

            if (member is FieldInfo fieldInfo)
            {
                return (IEnumerable)fieldInfo.GetValue(instance);
            }

            throw new Exception($"No supported MemberType: {member.MemberType}");
        }
    }
}