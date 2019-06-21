using ObjectApproval;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ApprovalTests.Reporters;
using ApprovalTests.Namers;


[TestClass]
[UseReporter(typeof(DiffReporter))]
[UseApprovalSubdirectory("Data")]
public class ParallelTests
{
    [TestInitialize]
    public void Setup()
    {
        SerializerBuilder.Reset();
    }

    private class TestObject
    {
        public int One { get; set; } = 1;
        public int Two { get; set; } = 2;
        public int Three { get; set; } = 3;
        public int Four { get; set; } = 4;
        public int Five { get; set; } = 5;
        public int Six { get; set; } = 6;
        public int Seven { get; set; } = 7;
        public int Eight { get; set; } = 8;
    }

    [TestMethod]
    public void One()
    {
        RunTest("One");
    }

    [TestMethod]
    public void Two()
    {
        RunTest("Two");
    }

    [TestMethod]
    public void Three()
    {
        RunTest("Three");
    }

    [TestMethod]
    public void Four()
    {
        RunTest("Four");
    }

    [TestMethod]
    public void Five()
    {
        RunTest("Five");
    }

    [TestMethod]
    public void Six()
    {
        RunTest("Six");
    }

    [TestMethod]
    public void Seven()
    {
        RunTest("Seven");
    }

    [TestMethod]
    public void Eight()
    {
        RunTest("Eight");
    }

    public void RunTest(string memberToIgnore)
    {
        SerializerBuilder.IgnoreMember(typeof(TestObject), memberToIgnore);

        var testObject = new TestObject();

        using (ApprovalResults.ForScenario("Parallel.Ignore." + memberToIgnore))
        {
            ObjectApprover.VerifyWithJson(testObject);
        }
    }
}