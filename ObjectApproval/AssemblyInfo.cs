using System.Reflection;
using ApprovalTests.Reporters;

[assembly: AssemblyTitle("ObjectApproval")]
[assembly: AssemblyProduct("ObjectApproval")]
[assembly: AssemblyCopyright("Copyright ©  2014")]
[assembly: AssemblyVersion("0.2.0")]
[assembly: AssemblyFileVersion("0.2.0")]



[assembly: UseReporter(typeof(ClipboardReporter), typeof(DiffReporter))]