using System;
using Newtonsoft.Json;

namespace ObjectApproval
{
    public static partial class ObjectApprover
    {
        [ObsoleteEx(
            RemoveInVersion = "10.0.0",
            ReplacementTypeOrMember = "Verify")]
        public static void VerifyWithJson(object target)
        {
            Verify(target, null);
        }

        [ObsoleteEx(
            RemoveInVersion = "10.0.0",
            ReplacementTypeOrMember = "Verify")]
        public static void VerifyWithJson(
            object target,
            Func<string, string> scrubber = null)
        {
            Verify(target, scrubber);
        }

        [ObsoleteEx(
            RemoveInVersion = "10.0.0",
            ReplacementTypeOrMember = "Verify")]
        public static void VerifyWithJson(
            object target,
            Func<string, string> scrubber = null,
            JsonSerializerSettings jsonSerializerSettings = null)
        {
            Verify(target, scrubber, jsonSerializerSettings);
        }

        [ObsoleteEx(
            RemoveInVersion = "10.0.0",
            ReplacementTypeOrMember = "Verify")]
        public static void VerifyWithJson(
            object target,
            bool ignoreEmptyCollections = true,
            bool scrubGuids = true,
            bool scrubDateTimes = true,
            bool ignoreFalse = true,
            Func<string, string> scrubber = null)
        {
            Verify(target, ignoreEmptyCollections, scrubGuids, scrubDateTimes, ignoreFalse, scrubber);
        }
    }
}