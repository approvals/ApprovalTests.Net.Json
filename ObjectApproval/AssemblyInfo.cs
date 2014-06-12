using System.Reflection;
using ApprovalTests.Reporters;

[assembly: AssemblyTitle("ObjectApproval")]
[assembly: AssemblyProduct("ObjectApproval")]
[assembly: AssemblyCopyright("Copyright ©  2014")]
[assembly: AssemblyVersion("0.2.1")]
[assembly: AssemblyFileVersion("0.2.1")]



[assembly: UseReporter(typeof(ClipboardReporter), typeof(DiffReporter))]