using ApprovalTests.Reporters;
using Xunit;
using ObjectApproval;

[assembly: UseReporter(typeof(AllFailingTestsClipboardReporter), typeof(DiffReporter))]
//[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, DisableTestParallelization = true)]
[assembly: ObjectApprovalBehavior(SerializerThreadingMode = SerializerThreadingMode.MultiThreaded)]