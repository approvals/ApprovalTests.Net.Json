using System.Collections.Generic;
using ApprovalTests;
using MyNamespace;
using ObjectApproval;
using Xunit;

public class CSharpTypeNameConverterTests
{
    [Fact]
    public void Simple()
    {
        Approvals.Verify(CSharpTypeNameConverter.GetName(typeof(string)));
    }

    [Fact]
    public void Nested()
    {
        Approvals.Verify(CSharpTypeNameConverter.GetName(typeof(TargetWithNested)));
    }

    [Fact]
    public void Nullable()
    {
        Approvals.Verify(CSharpTypeNameConverter.GetName(typeof(int?)));
    }

    [Fact]
    public void Array()
    {
        Approvals.Verify(CSharpTypeNameConverter.GetName(typeof(int[])));
    }

    [Fact]
    public void List()
    {
        Approvals.Verify(CSharpTypeNameConverter.GetName(typeof(List<TargetWithNamespace>)));
    }

    [Fact]
    public void ListOfArray()
    {
        Approvals.Verify(CSharpTypeNameConverter.GetName(typeof(List<TargetWithNamespace[]>)));
    }

    [Fact]
    public void ArrayOfList()
    {
        Approvals.Verify(CSharpTypeNameConverter.GetName(typeof(List<TargetWithNamespace>[])));
    }

    public class TargetWithNested{}
}

namespace MyNamespace
{
    public class TargetWithNamespace{}
}