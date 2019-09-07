using ObjectApproval;
using Xunit;
using Xunit.Abstractions;

public class DelegateConverterTests :
    XunitApprovalBase
{
    [Fact]
    public void Simple()
    {
        var cleaned1 = DelegateConverter.CleanMethodName("Void <SetProperties>b__100_2(System.String, Message.Importance)");
        Assert.Equal("Void SetProperties(System.String, Message.Importance)", cleaned1);
        var cleaned2 = DelegateConverter.CleanMethodName("Void SetProperties(System.String, Message.Importance)");
        Assert.Equal("Void SetProperties(System.String, Message.Importance)", cleaned2);
    }

    public DelegateConverterTests(ITestOutputHelper output) :
        base(output)
    {
    }
}