using System;
using ObjectApproval.Reflection;

namespace ObjectApproval
{
    class EnumSupportedStrategy : PocoJsonSerializerStrategy
    {
        protected override object SerializeEnum(Enum p)
        {
            return p.ToString();
        }

        public override object DeserializeObject(object value, Type type)
        {
            var stringValue = value as string;
            if (stringValue != null)
            {
                if (type.IsEnum)
                {
                    return Enum.Parse(type, stringValue);
                }

                if (ReflectionUtils.IsNullableType(type))
                {
                    var underlyingType = Nullable.GetUnderlyingType(type);
                    if (underlyingType.IsEnum)
                    {
                        return Enum.Parse(underlyingType, stringValue);
                    }
                }
            }

            return base.DeserializeObject(value, type);
        }
    }
}