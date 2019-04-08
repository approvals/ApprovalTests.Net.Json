using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

static class ReflectionHelpers
{
    public static bool IsCollection(this Type type)
    {
        Guard.AgainstNull(type, nameof(type));
        return type.GetInterfaces()
            .Any(x => x.IsGenericType &&
                      x.GetGenericTypeDefinition() == typeof(ICollection<>));
    }

    public static T GetValue<T>(this MemberInfo member, object instance)
    {
        // this value could be in a public field or public property
        if (member is PropertyInfo propertyInfo)
        {
            return (T) propertyInfo.GetValue(instance, null);
        }

        if (member is FieldInfo fieldInfo)
        {
            return (T) fieldInfo.GetValue(instance);
        }

        throw new Exception($"No supported MemberType: {member.MemberType}");
    }
}