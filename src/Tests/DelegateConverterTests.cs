using ObjectApproval;
using Xunit;

public class DelegateConverterTests
{
    [Fact]
    public void Simple()
    {
        var cleaned = DelegateConverter.CleanMethodName("Void <SetProperties>b__100_2(System.String, Message.Importance)");
        Assert.Equal("Void SetProperties(System.String, Message.Importance", cleaned);
    }
}