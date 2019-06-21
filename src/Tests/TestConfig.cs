using ApprovalTests.Reporters;
using ObjectApproval;

[assembly: UseReporter(typeof(AllFailingTestsClipboardReporter), typeof(DiffReporter))]
[assembly: ObjectApprovalBehavior(SerializerThreadingMode = SerializerThreadingMode.MultiThreaded)]