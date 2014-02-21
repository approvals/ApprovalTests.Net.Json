using System.Reflection;
using ApprovalTests.Reporters;

[assembly: AssemblyTitle("ObjectApproval")]
[assembly: AssemblyProduct("ObjectApproval")]
[assembly: AssemblyCopyright("Copyright ©  2014")]
[assembly: AssemblyVersion("0.1.4")]
[assembly: AssemblyFileVersion("0.1.4")]



[assembly: UseReporter(typeof(ClipboardReporter), typeof(DiffReporter))]