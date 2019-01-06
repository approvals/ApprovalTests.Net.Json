using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ObjectApproval
{
    public class CustomValueProvider : IValueProvider
    {
        IValueProvider inner;
        Type propertyType;

        public CustomValueProvider(IValueProvider inner, Type propertyType)
        {
            this.inner = inner;
            this.propertyType = propertyType;
        }

        public void SetValue(object target, object value)
        {
            throw new NotImplementedException();
        }

        public object GetValue(object target)
        {
            try
            {
                return inner.GetValue(target);
            }
            catch (JsonSerializationException exception)
            {
                if (exception.InnerException is NotImplementedException ||
                    exception.InnerException is NotSupportedException)
                {
                    return GetDefault();
                }

                throw;
            }
        }

        object GetDefault()
        {
            if (propertyType.IsValueType)
            {
                return Activator.CreateInstance(propertyType);
            }

            return null;
        }
    }
}