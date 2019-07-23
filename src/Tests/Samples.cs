using System;
using Newtonsoft.Json;
using ObjectApproval;
using Xunit;
using Xunit.Abstractions;

public class Samples:
    XunitLoggingBase
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
            scrubber: s => s.Replace("0x00000000000007D3", "TheRowVersion"));

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

    [Fact(Skip = "explicit")]
    public void ScopedSerializer()
    {
        #region ScopedSerializer

        var person = new Person
        {
            GivenNames = "John",
            FamilyName = "Smith",
            Dob = new DateTime(2000, 10, 1),
        };
        var serializerSettings = SerializerBuilder.BuildSettings(scrubDateTimes: false);
        serializerSettings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
        ObjectApprover.VerifyWithJson(person, jsonSerializerSettings: serializerSettings);

        #endregion
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
        public DateTime Dob;
    }

    class Address
    {
        public string Street;
        public string Country;
        public string Suburb;
    }

    public Samples(ITestOutputHelper output) :
        base(output)
    {
    }
}