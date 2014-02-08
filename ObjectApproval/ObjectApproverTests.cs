using NUnit.Framework;
using ObjectApproval;

[TestFixture]
public class ObjectApproverTests
{
    [Test]
    public void Example()
    {
        var person = new Person
            {
                GivenNames = "John",
                FamilyName = "Smith",
                Spouse = "Jill",
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
    }

    class Address
    {
        public string Street;
        public string Suburb;
        public string Country;
    }
}