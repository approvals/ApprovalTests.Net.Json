#if !NETSTANDARD
using System.Reflection;
using System;
using System.Linq.Expressions;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;

public static partial class ObjectApprover
{
    public static void VerifyTuple(Expression<Func<ITuple>> expression)
    {
        var dictionary = ExpressionToDictionary(expression);
        Verify(dictionary, null);
    }

    public static void VerifyTuple(Expression<Func<ITuple>> expression, Func<string, string> scrubber = null)
    {
        var dictionary = ExpressionToDictionary(expression);
        Verify(dictionary, scrubber, null);
    }

    public static void VerifyTuple(Expression<Func<ITuple>> expression, Func<string, string> scrubber = null, JsonSerializerSettings jsonSerializerSettings = null)
    {
        var dictionary = ExpressionToDictionary(expression);
        Verify(dictionary, scrubber,jsonSerializerSettings);
    }

    public static void VerifyTuple(
        Expression<Func<ITuple>> expression,
        bool ignoreEmptyCollections = true,
        bool scrubGuids = true,
        bool scrubDateTimes = true,
        bool ignoreFalse = true,
        Func<string, string> scrubber = null)
    {
        var dictionary = ExpressionToDictionary(expression);
        Verify(dictionary, ignoreEmptyCollections, scrubGuids, scrubDateTimes, ignoreFalse, scrubber);
    }

    static Dictionary<string, object> ExpressionToDictionary(Expression<Func<ITuple>> expression)
    {
        var unaryExpression = (UnaryExpression) expression.Body;
        var methodCallExpression = (MethodCallExpression) unaryExpression.Operand;
        var method = methodCallExpression.Method;
        var attribute = ReadTupleElementNamesAttribute(method);
        var dictionary = new Dictionary<string, object>();
        var result = expression.Compile().Invoke();
        for (var index = 0; index < attribute.TransformNames.Count; index++)
        {
            var transformName = attribute.TransformNames[index];
            dictionary.Add(transformName, result[index]);
        }

        return dictionary;
    }

    static TupleElementNamesAttribute ReadTupleElementNamesAttribute(MethodInfo method)
    {
        var attribute = (TupleElementNamesAttribute) method.ReturnTypeCustomAttributes.GetCustomAttributes(typeof(TupleElementNamesAttribute), false).SingleOrDefault();
        if (attribute == null)
        {
            throw new Exception(nameof(VerifyTuple) + " is only to be used on methods that return a tuple.");
        }

        return attribute;
    }
}
#endif