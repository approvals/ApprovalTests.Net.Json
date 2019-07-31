using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using ApprovalTests;
using Newtonsoft.Json;

namespace ObjectApproval
{
    public static partial class ObjectApprover
    {
        public static void Verify(object target)
        {
            Verify(target, null);
        }
        public static void Verify(Expression target)
        {
            Verify(target, null);
        }
#if !NETSTANDARD
        public static void VerifyTuple(Expression<Func<ITuple>> expression)
        {
            var unaryExpression = (UnaryExpression) expression.Body;
            var methodCallExpression = (MethodCallExpression) unaryExpression.Operand;
            var method = methodCallExpression.Method;
            var attribute = (TupleElementNamesAttribute) method.ReturnTypeCustomAttributes.GetCustomAttributes(typeof(TupleElementNamesAttribute),false).SingleOrDefault();
            if (attribute == null)
            {
                throw new Exception(nameof(VerifyTuple) + " is only to be used on methods that return a tuple.");
            }

            var dictionary = new Dictionary<string, object>();
            var result = expression.Compile().Invoke();
            for (var index = 0; index < attribute.TransformNames.Count; index++)
            {
                var transformName = attribute.TransformNames[index];
                dictionary.Add(transformName, result[index]);
            }
            Verify(dictionary, null);
        }

#endif
        public static void Verify(object target, Func<string, string> scrubber = null)
        {
            Verify(target, scrubber, null);
        }

        public static void Verify(object target, Func<string, string> scrubber = null, JsonSerializerSettings jsonSerializerSettings = null)
        {
            var formatJson = AsFormattedJson(target, jsonSerializerSettings);
            if (scrubber == null)
            {
                scrubber = s => s;
            }

            Approvals.Verify(formatJson, scrubber);
        }

        public static void Verify(
            object target,
            bool ignoreEmptyCollections = true,
            bool scrubGuids = true,
            bool scrubDateTimes = true,
            bool ignoreFalse = true,
            Func<string, string> scrubber = null)
        {
            var settings = SerializerBuilder.BuildSettings(ignoreEmptyCollections, scrubGuids, scrubDateTimes, ignoreFalse);
            var formatJson = AsFormattedJson(target, settings);
            if (scrubber == null)
            {
                scrubber = s => s;
            }

            Approvals.Verify(formatJson, scrubber);
        }

        public static string AsFormattedJson(object target, JsonSerializerSettings jsonSerializerSettings = null)
        {
            var type = target.GetType();
            var serializer = GetJsonSerializer(jsonSerializerSettings);
            var builder = new StringBuilder();
            using (var stringWriter = new StringWriter(builder))
            using (var writer = new JsonTextWriter(stringWriter))
            {
                writer.QuoteChar = '\'';
                writer.QuoteName = false;
                serializer.Serialize(writer, target);
            }

            builder.Replace(@"\\", @"\");
            return builder.ToString();
        }

        static JsonSerializer GetJsonSerializer(JsonSerializerSettings jsonSerializerSettings)
        {
            if (jsonSerializerSettings == null)
            {
                return JsonSerializer.Create(SerializerBuilder.BuildSettings());
            }

            return JsonSerializer.Create(jsonSerializerSettings);
        }

    }
}