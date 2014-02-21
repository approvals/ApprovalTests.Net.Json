using System;
using ApprovalTests;

namespace ObjectApproval
{
    public static class ObjectApprover
    {
        public static void VerifyWithJson(object target)
        {
            VerifyWithJson(target, s => s);
        }

        public static void VerifyWithJson(object target, Func<string,string> scrubber)
        {
            var serializeObject = SimpleJson.SerializeObject(target);
            var formatJson = serializeObject.FormatJson().FixNewLines();
            Approvals.Verify(formatJson, s =>
            {
                var fixNewLines = s.FixNewLines();
                return scrubber(fixNewLines);
            });
        }
    }
}