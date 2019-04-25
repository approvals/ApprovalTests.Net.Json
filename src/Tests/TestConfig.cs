using ApprovalTests.Reporters;
using Xunit;

[assembly: UseReporter(typeof(AllFailingTestsClipboardReporter), typeof(DiffReporter))]
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, DisableTestParallelization = true)]