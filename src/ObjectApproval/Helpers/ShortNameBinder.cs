using System;
using Newtonsoft.Json.Serialization;

namespace ObjectApproval
{
    public class ShortNameBinder :
        ISerializationBinder
    {
        public void BindToName(Type serializedType, out string? assemblyName, out string? typeName)
        {
            assemblyName = "";
            typeName = TypeNameConverter.GetName(serializedType);
        }

        public Type BindToType(string? assemblyName, string typeName)
        {
            throw new Exception();
        }
    }
}