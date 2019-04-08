using Newtonsoft.Json;
using ObjectApproval;
using Xunit;

public class Samples
{
    [Fact]
    public void Scrubber()
    {
        #region Scrubber

        var target = new ToBeScrubbed
        {
            RowVersion = "0x00000000000007D3"
        };

        ObjectApprover.VerifyWithJson(target,
            scrubber: s => s.Replace("0x00000000000007D3", "ThRowVersion"));

        #endregion
    }

    void ChangeDefaultsPerVerification(object target)
    {
        #region ChangeDefaultsPerVerification

        ObjectApprover.VerifyWithJson(target,
            ignoreEmptyCollections: false,
            scrubGuids: false,
            scrubDateTimes: false,
            ignoreFalse: false);

        #endregion
    }

    class ToBeScrubbed
    {
        public string RowVersion;
    }

    void Before()
    {
        #region Before

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

        #endregion
    }

    [Fact]
   public void Anon()
    {
        #region Anon

        var person1 = new Person
        {
            GivenNames = "John",
            FamilyName = "Smith"
        };
        var person2 = new Person
        {
            GivenNames = "Marianne",
            FamilyName = "Aguirre"
        };

        ObjectApprover.VerifyWithJson(
            new
            {
                person1,
                person2
            });

        #endregion
    }

    void ExtraSettings()
    {
        #region ExtraSettings

        SerializerBuilder.ExtraSettings =
            jsonSerializerSettings =>
            {
                jsonSerializerSettings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
                jsonSerializerSettings.TypeNameHandling = TypeNameHandling.All;
            };

        #endregion
    }

    void After()
    {
        #region After

        var person = new Person
        {
            GivenNames = "John",
            FamilyName = "Smith",
            Spouse = "Jill",
            Address = new Address
            {
                Street = "1 Puddle Lane",
                Suburb = "Gotham",
                Country = "USA"
            }
        };

        ObjectApprover.VerifyWithJson(person);

        #endregion
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
        public string Country;
        public string Suburb;
    }
}