using System;
using System.CodeDom;
using Microsoft.CSharp;
using Newtonsoft.Json.Serialization;

namespace ObjectApproval
{
    public class ShortNameBinder : ISerializationBinder
    {
        static CSharpCodeProvider codeDomProvider = new CSharpCodeProvider();

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = GetOriginalName(serializedType);
        }

        public Type BindToType(string assemblyName, string typeName)
        {
            throw new Exception();
        }

        static string GetOriginalName(Type type)
        {
            var typeName = type.FullName.Replace(type.Namespace + ".", "");
            var reference = new CodeTypeReference(typeName);
            return codeDomProvider.GetTypeOutput(reference);
        }
    }
}