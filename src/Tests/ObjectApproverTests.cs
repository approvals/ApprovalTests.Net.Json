using System;
using System.Collections.Generic;
using ObjectApproval;
using Xunit;

public class ObjectApproverTests
{
    static ObjectApproverTests()
    {
        StringScrubber.AddExtraDatetimeFormat("F");
        StringScrubber.AddExtraDatetimeOffsetFormat("F");
    }

    [Fact]
    public void ShouldReUseGuid()
    {
        #region guid

        var guid = Guid.NewGuid();
        var target = new GuidTarget
        {
            Guid = guid,
            GuidNullable = guid,
            GuidString = guid.ToString(),
            OtherGuid = Guid.NewGuid(),
        };

        ObjectApprover.VerifyWithJson(target);

        #endregion
    }

    [Fact]
    public void IgnoreType()
    {
        #region AddIgnore

        var target = new IgnoreTypeTarget
        {
            ToIgnore = new ToIgnore
            {
                Property = "Value"
            }
        };
        SerializerBuilder.AddIgnore<ToIgnore>();

        ObjectApprover.VerifyWithJson(target);

        #endregion
    }

    class IgnoreTypeTarget
    {
        public ToIgnore ToIgnore { get; set; }
    }

    class ToIgnore
    {
        public string Property { get; set; }
    }

    [Fact]
    public void IgnoreMemberByExpression()
    {
        #region IgnoreMemberByExpression
        var target = new IgnoreExplicitTarget
        {
            Include = "Value",
            Field = "Value",
            Property = "Value"
        };
        SerializerBuilder.AddIgnore<IgnoreExplicitTarget>(x => x.Property);
        SerializerBuilder.AddIgnore<IgnoreExplicitTarget>(x => x.Field);
        SerializerBuilder.AddIgnore<IgnoreExplicitTarget>(x => x.GetOnlyProperty);
        SerializerBuilder.AddIgnore<IgnoreExplicitTarget>(x => x.PropertyThatThrows);
        ObjectApprover.VerifyWithJson(target);
        #endregion
    }

    [Fact]
    public void IgnoreMemberByName()
    {
        #region IgnoreMemberByName

        var target = new IgnoreExplicitTarget
        {
            Include = "Value",
            Field = "Value",
            Property = "Value"
        };
        var type = typeof(IgnoreExplicitTarget);
        SerializerBuilder.AddIgnore(type, "Property");
        SerializerBuilder.AddIgnore(type, "Field");
        SerializerBuilder.AddIgnore(type, "GetOnlyProperty");
        SerializerBuilder.AddIgnore(type, "PropertyThatThrows");
        ObjectApprover.VerifyWithJson(target);

        #endregion
    }

    class IgnoreExplicitTarget
    {
        public string Include { get; set; }
        public string Property { get; set; }
        public string GetOnlyProperty => "asd";
        public string PropertyThatThrows => throw new Exception();
        public string Field;
    }

    [Fact]
    public void NotImplementedExceptionProp()
    {
        var target = new WithNotImplementedException();

        ObjectApprover.VerifyWithJson(target);
    }

    class WithNotImplementedException
    {
        public Guid NotImplementedExceptionObsoleteProperty => throw new NotImplementedException();
    }

    [Fact]
    public void NotSupportedExceptionProp()
    {
        var target = new WithNotSupportedException();

        ObjectApprover.VerifyWithJson(target);
    }

    class WithNotSupportedException
    {
        public Guid NotImplementedExceptionProperty => throw new NotSupportedException();
    }

    [Fact]
    public void WithObsoleteProp()
    {
        var target = new WithObsolete();

        ObjectApprover.VerifyWithJson(target);
    }

    class WithObsolete
    {
        Guid obsoleteProperty;

        [Obsolete]
        public Guid ObsoleteProperty
        {
            get { throw new NotImplementedException(); }
            set => obsoleteProperty = value;
        }
    }

    [Fact]
    public void OnlySpecificDates()
    {
        var target = new NotDatesTarget
        {
            NotDate = "1.2.3"
        };

        ObjectApprover.VerifyWithJson(target);
    }

    public class NotDatesTarget
    {
        public string NotDate;
    }

    [Fact]
    public void ShouldScrubGuid()
    {
        var target = new GuidTarget
        {
            Guid = Guid.NewGuid(),
            GuidNullable = Guid.NewGuid(),
            GuidString = Guid.NewGuid().ToString(),
        };

        ObjectApprover.VerifyWithJson(target);
    }

    [Fact]
    public void ShouldIgnoreEmptyList()
    {
        var target = new CollectionTarget
        {
            DictionaryProperty = new Dictionary<int, string>(),
            ListProperty = new List<string>()
        };

        ObjectApprover.VerifyWithJson(target);
    }

    public class CollectionTarget
    {
        public Dictionary<int, string> DictionaryProperty;
        public List<string> ListProperty;
    }

    [Fact]
    public void ShouldIgnoreGuidDefaults()
    {
        var target = new GuidTarget();

        ObjectApprover.VerifyWithJson(target);
    }

    public class GuidTarget
    {
        public Guid Guid;
        public Guid? GuidNullable;
        public string GuidString;
        public Guid OtherGuid;
    }

    [Fact]
    public void ShouldReUseDatetime()
    {
        #region Date

        var dateTime = DateTime.Now;
        var dateTimeOffset = DateTimeOffset.Now;
        var target = new DateTimeTarget
        {
            DateTime = dateTime,
            DateTimeNullable = dateTime,
            DateTimeString = dateTime.ToString("F"),
            DateTimeOffset = dateTimeOffset,
            DateTimeOffsetNullable = dateTimeOffset,
            DateTimeOffsetString = dateTimeOffset.ToString("F"),
        };

        ObjectApprover.VerifyWithJson(target);

        #endregion
    }

    [Fact]
    public void ShouldScrubDatetime()
    {
        var dateTime = DateTime.Now;
        var dateTimeOffset = DateTimeOffset.Now;
        var target = new DateTimeTarget
        {
            DateTime = dateTime,
            DateTimeNullable = dateTime.AddDays(1),
            DateTimeString = dateTime.AddDays(2).ToString("F"),
            DateTimeOffset = dateTimeOffset,
            DateTimeOffsetNullable = dateTimeOffset.AddDays(1),
            DateTimeOffsetString = dateTimeOffset.AddDays(2).ToString("F"),
        };

        ObjectApprover.VerifyWithJson(target);
    }

    [Fact]
    public void ShouldIgnoreDatetimeDefaults()
    {
        var target = new DateTimeTarget();

        ObjectApprover.VerifyWithJson(target);
    }

    public class DateTimeTarget
    {
        public DateTime DateTime;
        public DateTime? DateTimeNullable;
        public DateTimeOffset DateTimeOffset;
        public DateTimeOffset? DateTimeOffsetNullable;
        public string DateTimeString;
        public string DateTimeOffsetString;
    }

    [Fact]
    public void ExampleNonDefaults()
    {
        var person = new Person
        {
            Id = new Guid("ebced679-45d3-4653-8791-3d969c4a986c"),
            Title = Title.Mr,
            GivenNames = "John",
            FamilyName = "Smith",
            Dob = DateTime.MaxValue,
            Spouse = "Jill",
            Children = new List<string> {"Sam", "Mary"},
            Address = new Address
            {
                Street = "1 Puddle Lane",
                Country = "USA"
            }
        };

        ObjectApprover.VerifyWithJson(person, false, false, false, s => s.Replace("Lane", "Street"));
    }

    [Fact]
    public void Example()
    {
        var person = new Person
        {
            Id = Guid.NewGuid(),
            Title = Title.Mr,
            GivenNames = "John",
            FamilyName = "Smith",
            Dob = DateTime.Now,
            Spouse = "Jill",
            Children = new List<string> {"Sam", "Mary"},
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
        public List<string> Children;
        public Title Title;
        public DateTime Dob;
        public Guid Id;
    }

    class Address
    {
        public string Street;
        public string Suburb;
        public string Country;
    }

    enum Title
    {
        Mr
    }
}
