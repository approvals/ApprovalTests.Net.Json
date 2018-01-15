using System.Collections.Generic;
using ApprovalTests.Reporters;
using ObjectApproval;
using Xunit;

[assembly: UseReporter(typeof(ClipboardReporter), typeof(DiffReporter))]

public class ObjectApproverTests
{
    [Fact]
    public void Example()
    {
        var person = new Person
            {
                Title = Title.Mr,
                GivenNames = "John",
                FamilyName = "Smith",
                Spouse = "Jill",
                Childres = new List<string> {"Sam", "Mary"},
                Address = new Address
                    {
                        Street = "1 Puddle Lane",
                        Country = "USA"
                    }
            };

        ObjectApprover.VerifyWithJson(person);
    }

    class Person
    {
        public string GivenNames;
        public string FamilyName;
        public string Spouse;
        public Address Address;
        public List<string> Childres;
        public Title Title;
    }

    class Address
    {
        public string Street;
        public string Suburb;
        public string Country;
    }
}

public enum Title
{
    Mr
}