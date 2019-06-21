using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectApproval;

[assembly: Parallelize(Workers = 0, Scope = ExecutionScope.MethodLevel)]
[assembly: ObjectApprovalBehavior(SerializerThreadingMode = SerializerThreadingMode.MultiThreaded)]