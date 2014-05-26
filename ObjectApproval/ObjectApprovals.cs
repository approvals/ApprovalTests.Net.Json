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
            var serializeObject = SimpleJson.SerializeObject(target, new EnumSupportedStrategy());
            var formatJson = serializeObject.FormatJson().FixNewLines();
            Approvals.Verify(formatJson, s => scrubber(s).FixNewLines());
        }
    }
}