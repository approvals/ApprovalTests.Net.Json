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

		public static void VerifyWithJson(object target, Func<string, string> scrubber)
		{
			var formatJson = AsFormattedJson(target);
			Approvals.Verify(formatJson, s => scrubber(s).FixNewLines());
		}

		public static string AsFormattedJson(object target)
		{
			var serializeObject = SimpleJson.SerializeObject(target, new EnumSupportedStrategy());
			var formatJson = serializeObject.FormatJson().FixNewLines();
			return formatJson;
		}
	}
}