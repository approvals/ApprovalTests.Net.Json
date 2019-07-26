using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ObjectApproval;
using Xunit;
using Xunit.Abstractions;

public class ObjectApproverTests :
    XunitLoggingBase
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

        ObjectApprover.Verify(target);

        #endregion
    }

    [Fact]
    public void ShouldUseShortTypeName()
    {
        #region type

        var foo = new {x = 1};
        var target = new TypeTarget
        {
            Type = GetType(),
            Dynamic = foo.GetType(),
        };

        ObjectApprover.Verify(target);

        #endregion
    }

    [Fact]
    public void IgnoreInstance()
    {
        #region AddIgnoreInstance

        // Done on static startup
        SerializerBuilder.IgnoreInstance<Instance>(x => x.Property == "Ignore");

        // Done as part of test
        var target = new IgnoreInstanceTarget
        {
            ToIgnore = new Instance
            {
                Property = "Ignore"
            },
            ToInclude = new Instance
            {
                Property = "Include"
            },
        };
        ObjectApprover.Verify(target);

        #endregion
    }

    [Fact]
    public void IgnoreInstanceReset()
    {
        SerializerBuilder.IgnoreInstance<Instance>(x => x.Property == "Ignore");

        SerializerBuilder.Reset();

        var target = new IgnoreInstanceTarget
        {
            ToIgnore = new Instance
            {
                Property = "Ignore"
            },
            ToInclude = new Instance
            {
              Property = "Include"
            }
        };

        ObjectApprover.Verify(target);
    }

    class IgnoreInstanceTarget
    {
        public Instance ToIgnore;
        public Instance ToInclude;
    }

    class Instance
    {
        public string Property;
    }

    [Fact]
    public void IgnoreType()
    {
        #region AddIgnoreType

        // Done on static startup
        SerializerBuilder.IgnoreMembersWithType<ToIgnore>();

        // Done as part of test
        var target = new IgnoreTypeTarget
        {
            ToIgnore = new ToIgnore
            {
                Property = "Value"
            },
            ToInclude = new ToInclude
            {
                Property = "Value"
            }
        };
        ObjectApprover.Verify(target);

        #endregion
    }

    [Fact]
    public void IgnoreTypeReset()
    {
        SerializerBuilder.IgnoreMembersWithType<ToIgnore>();

        SerializerBuilder.Reset();

        var target = new IgnoreTypeTarget
        {
            ToIgnore = new ToIgnore
            {
                Property = "Value"
            }
        };

        ObjectApprover.Verify(target);
    }

    class IgnoreTypeTarget
    {
        public ToIgnore ToIgnore;
        public ToInclude ToInclude;
    }

    class ToInclude
    {
        public string Property;
    }

    class ToIgnore
    {
        public string Property;
    }

    [Fact]
    public void IgnoreMemberByExpression()
    {
        #region IgnoreMemberByExpression

        // Done on static startup
        SerializerBuilder.IgnoreMember<IgnoreExplicitTarget>(x => x.Property);
        SerializerBuilder.IgnoreMember<IgnoreExplicitTarget>(x => x.Field);
        SerializerBuilder.IgnoreMember<IgnoreExplicitTarget>(x => x.GetOnlyProperty);
        SerializerBuilder.IgnoreMember<IgnoreExplicitTarget>(x => x.PropertyThatThrows);

        // Done as part of test
        var target = new IgnoreExplicitTarget
        {
            Include = "Value",
            Field = "Value",
            Property = "Value"
        };
        ObjectApprover.Verify(target);

        #endregion
    }

    [Fact]
    public void IgnoreMemberByName()
    {
        #region IgnoreMemberByName

        // Done on static startup
        var type = typeof(IgnoreExplicitTarget);
        SerializerBuilder.IgnoreMember(type, "Property");
        SerializerBuilder.IgnoreMember(type, "Field");
        SerializerBuilder.IgnoreMember(type, "GetOnlyProperty");
        SerializerBuilder.IgnoreMember(type, "PropertyThatThrows");

        // Done as part of test
        var target = new IgnoreExplicitTarget
        {
            Include = "Value",
            Field = "Value",
            Property = "Value"
        };
        ObjectApprover.Verify(target);

        #endregion
    }

    [Fact]
    public void IgnoreMemberReset()
    {
        var type = typeof(IgnoreExplicitTarget);
        SerializerBuilder.IgnoreMember(type, "Property");

        SerializerBuilder.Reset();

        SerializerBuilder.IgnoreMember(type, "PropertyThatThrows");

        var target = new IgnoreExplicitTarget
        {
            Property = "Value"
        };

        ObjectApprover.Verify(target);
    }

    class IgnoreExplicitTarget
    {
        public string Include;
        public string Property { get; set; }
        public string GetOnlyProperty => "asd";
        public string PropertyThatThrows => throw new Exception();
        public string Field;
    }

    [Fact]
    public void NotImplementedExceptionProp()
    {
        var target = new WithNotImplementedException();
        ObjectApprover.Verify(target);
    }

    class WithNotImplementedException
    {
        public Guid NotImplementedExceptionProperty => throw new NotImplementedException();
    }

    [Fact]
    public void CustomExceptionProp()
    {
        #region IgnoreMembersThatThrow

        // Done on static startup
        SerializerBuilder.IgnoreMembersThatThrow<CustomException>();

        // Done as part of test
        var target = new WithCustomException();
        ObjectApprover.Verify(target);

        #endregion
    }

    [Fact]
    public void IgnoreMembersThatThrowReset()
    {
        SerializerBuilder.IgnoreMembersThatThrow<CustomException>();
        SerializerBuilder.Reset();

        var target = new WithCustomException();

        // not ignoring the thrown exception will cause the serialization to fail
        Assert.Throws<JsonSerializationException>(() => ObjectApprover.Verify(target));
    }

    class WithCustomException
    {
        public Guid CustomExceptionProperty => throw new CustomException();
    }

    [Fact]
    public void ExceptionProps()
    {
        try
        {
            throw new Exception();
        }
        catch (Exception exception)
        {
            ObjectApprover.Verify(exception);
        }
    }

    [Fact]
    public void ExceptionProp()
    {
        SerializerBuilder.IgnoreMembersThatThrow<CustomException>();

        var target = new WithException();

        Assert.Throws<JsonSerializationException>(() => ObjectApprover.Verify(target));
    }

    class WithException
    {
        public Guid ExceptionProperty => throw new Exception();
    }

    internal class CustomException : Exception
    {
    }

    [Fact]
    public void ExceptionMessageProp()
    {
        #region IgnoreMembersThatThrowExpression

        // Done on static startup
        SerializerBuilder.IgnoreMembersThatThrow<Exception>(
            x => { return x.Message == "Ignore"; });

        // Done as part of test
        var target = new WithExceptionIgnoreMessage();
        ObjectApprover.Verify(target);

        #endregion
    }

    class WithExceptionIgnoreMessage
    {
        public Guid ExceptionMessageProperty => throw new Exception("Ignore");
    }

    [Fact]
    public void ExceptionNotIgnoreMessageProp()
    {
        SerializerBuilder.IgnoreMembersThatThrow<Exception>(x => x.Message == "Ignore");
        var target = new WithExceptionNotIgnoreMessage();

        Assert.Throws<JsonSerializationException>(() => ObjectApprover.Verify(target));
    }

    class WithExceptionNotIgnoreMessage
    {
        public Guid ExceptionMessageProperty => throw new Exception("NotIgnore");
    }

    [Fact]
    public void DelegateProp()
    {
        var target = new WithDelegate();
        ObjectApprover.Verify(target);
    }

    class WithDelegate
    {
        public Action DelegateProperty => () => { };
    }

    [Fact]
    public void NotSupportedExceptionProp()
    {
        var target = new WithNotSupportedException();
        ObjectApprover.Verify(target);
    }

    class WithNotSupportedException
    {
        public Guid NotImplementedExceptionProperty => throw new NotSupportedException();
    }

    [Fact]
    public void WithObsoleteProp()
    {
        var target = new WithObsolete();
        ObjectApprover.Verify(target);
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
    public void Escaping()
    {
        var target = new EscapeTarget
        {
            Property = @"\"
        };
        ObjectApprover.Verify(target);
    }

    public class EscapeTarget
    {
        public string Property;
    }

    [Fact]
    public void OnlySpecificDates()
    {
        var target = new NotDatesTarget
        {
            NotDate = "1.2.3"
        };
        ObjectApprover.Verify(target);
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
        ObjectApprover.Verify(target);
    }

    [Fact]
    public void ShouldIgnoreEmptyList()
    {
        var target = new CollectionTarget
        {
            DictionaryProperty = new Dictionary<int, string>(),
            ListProperty = new List<string>()
        };
        ObjectApprover.Verify(target);
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
        ObjectApprover.Verify(target);
    }

    public class GuidTarget
    {
        public Guid Guid;
        public Guid? GuidNullable;
        public string GuidString;
        public Guid OtherGuid;
    }

    public class TypeTarget
    {
        public Type Type;
        public Type Dynamic;
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

        ObjectApprover.Verify(target);

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

        ObjectApprover.Verify(target);
    }

    [Fact]
    public void ShouldIgnoreDatetimeDefaults()
    {
        var target = new DateTimeTarget();

        ObjectApprover.Verify(target);
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
            Dead = false,
            UnDead = null,
            Address = new Address
            {
                Street = "1 Puddle Lane",
                Country = "USA"
            }
        };

        ObjectApprover.Verify(
            person,
            ignoreEmptyCollections: false,
            scrubGuids: false,
            scrubDateTimes: false,
            ignoreFalse: false,
            scrubber: s => s.Replace("Lane", "Street"));
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

        ObjectApprover.Verify(person);
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
        public bool Dead;
        public bool? UnDead;
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

    [Fact(Skip = "explicit")]
    public void ShouldUseExtraSettings()
    {
        SerializerBuilder.ExtraSettings =
            settings =>
            {
                settings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
            };

        var person = new Person
        {
            Dob = new DateTime(1980, 5, 5, 1, 1, 1)
        };

        ObjectApprover.Verify(person, scrubDateTimes: false);
    }

    [Fact(Skip = "explicit")]
    public void ShouldUseExtraSettingsReset()
    {
        SerializerBuilder.ExtraSettings =
            settings =>
            {
                settings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
            };

        SerializerBuilder.Reset();

        var person = new Person
        {
            Dob = new DateTime(1980, 5, 5, 1, 1, 1)
        };

        ObjectApprover.Verify(person, scrubDateTimes: false);
    }


    public ObjectApproverTests(ITestOutputHelper output) :
        base(output)
    {
        SerializerBuilder.Reset(); // reset builder for a clean slate each run
    }
}