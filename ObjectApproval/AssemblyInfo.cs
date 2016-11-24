using System.Reflection;
using ApprovalTests.Reporters;

[assembly: AssemblyTitle("ObjectApproval")]
[assembly: AssemblyProduct("ObjectApproval")]
[assembly: AssemblyVersion("1.5.0")]
[assembly: AssemblyFileVersion("1.5.0")]
[assembly: UseReporter(typeof(ClipboardReporter), typeof(DiffReporter))]