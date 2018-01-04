using System.Reflection;
using ApprovalTests.Reporters;

[assembly: AssemblyTitle("ObjectApproval")]
[assembly: AssemblyProduct("ObjectApproval")]
[assembly: AssemblyVersion("2.0.0")]
[assembly: AssemblyFileVersion("2.0.0")]
[assembly: UseReporter(typeof(ClipboardReporter), typeof(DiffReporter))]