using System.Collections.Generic;
using System.Linq;
using ApprovalTests;
using MyNamespace;
using ObjectApproval;
using Xunit;

public class TypeNameConverterTests
{
    [Fact]
    public void Simple()
    {
        Approvals.Verify(TypeNameConverter.GetName(typeof(string)));
    }

    [Fact]
    public void Nested()
    {
        Approvals.Verify(TypeNameConverter.GetName(typeof(TargetWithNested)));
    }

    [Fact]
    public void Nullable()
    {
        Approvals.Verify(TypeNameConverter.GetName(typeof(int?)));
    }

    [Fact]
    public void Array()
    {
        Approvals.Verify(TypeNameConverter.GetName(typeof(int[])));
    }

    [Fact]
    public void List()
    {
        Approvals.Verify(TypeNameConverter.GetName(typeof(List<TargetWithNamespace>)));
    }

    [Fact]
    public void Enumerable()
    {
        Approvals.Verify(TypeNameConverter.GetName(typeof(IEnumerable<TargetWithNamespace>)));
    }

    [Fact]
    public void Dynamic()
    {
        Approvals.Verify(TypeNameConverter.GetName(new{Name="foo"}.GetType()));
    }

    [Fact]
    public void RuntimeEnumerable()
    {
        Approvals.Verify(TypeNameConverter.GetName(MethodWithYield().GetType()));
    }

    [Fact]
    public void RuntimeEnumerableDynamic()
    {
        Approvals.Verify(TypeNameConverter.GetName(MethodWithYieldDynamic().GetType()));
    }

    [Fact]
    public void RuntimeEnumerableWithSelect()
    {
        Approvals.Verify(TypeNameConverter.GetName(MethodWithYield().Select(x=>x!=null).GetType()));
    }

    [Fact]
    public void RuntimeEnumerableDynamicWithSelect()
    {
        Approvals.Verify(TypeNameConverter.GetName(MethodWithYieldDynamic().Select(x=>x!=null).GetType()));
    }

    [Fact]
    public void RuntimeEnumerableDynamicWithInnerSelect()
    {
        Approvals.Verify(TypeNameConverter.GetName(MethodWithYield().Select(x=>new {X=x.ToString()}).GetType()));
    }

    [Fact]
    public void EnumerableOfArray()
    {
        Approvals.Verify(TypeNameConverter.GetName(typeof(IEnumerable<TargetWithNamespace[]>)));
    }

    IEnumerable<TargetWithNamespace> MethodWithYield()
    {
        yield return new TargetWithNamespace();
    }
    IEnumerable<dynamic> MethodWithYieldDynamic()
    {
        yield return new {X="1"};
    }

    [Fact]
    public void ListOfArray()
    {
        Approvals.Verify(TypeNameConverter.GetName(typeof(List<TargetWithNamespace[]>)));
    }

    [Fact]
    public void ArrayOfList()
    {
        Approvals.Verify(TypeNameConverter.GetName(typeof(List<TargetWithNamespace>[])));
    }

    public class TargetWithNested{}
}

namespace MyNamespace
{
    public class TargetWithNamespace{}
}