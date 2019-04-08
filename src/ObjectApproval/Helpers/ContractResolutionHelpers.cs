using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace ObjectApproval
{
    public static class ContractResolutionHelpers
    {
        public static void SkipEmptyCollections(this JsonProperty property, MemberInfo member)
        {
            Guard.AgainstNull(property, nameof(property));
            Guard.AgainstNull(member, nameof(member));
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
                var collection = member.GetCollection(instance);

                if (collection == null)
                {
                    // if the list is null, we defer the decision to NullValueHandling
                    return true;
                }

                // check to see if there is at least one item in the Enumerable
                return collection.GetEnumerator().MoveNext();
            };
        }
    }
}