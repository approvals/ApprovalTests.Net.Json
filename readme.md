# <img src="https://raw.github.com/SimonCropp/ObjectApproval/master/icon.png" height="40px"> ObjectApproval

Extends [ApprovalTests](https://github.com/approvals/ApprovalTests.Net) to allow simple approval of complex models using [Json.net](https://www.newtonsoft.com/json).

**This project is supported by the community via [Patreon sponsorship](https://www.patreon.com/join/simoncropp). If you are using this project to deliver business value or build commercial software it is expected that you will provide support [via Patreon](https://www.patreon.com/join/simoncropp).**


## The NuGet package [![NuGet Status](http://img.shields.io/nuget/v/ObjectApproval.svg?style=flat)](https://www.nuget.org/packages/ObjectApproval/)

https://nuget.org/packages/ObjectApproval/

    PM> Install-Package ObjectApproval


## Usage

Assuming you have previously verified and approved using this. 

<!-- snippet: before -->
```cs
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
```
<sup>[snippet source](/src/Tests/Samples.cs#L41-L57)</sup>
<!-- endsnippet -->

Then you attempt to verify this 

<!-- snippet: after -->
```cs
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
```
<sup>[snippet source](/src/Tests/Samples.cs#L102-L119)</sup>
<!-- endsnippet -->

The serialized json version of these will then be compared and you will be displayed the differences in the diff tool you have asked ApprovalTests to use. For example:

![SampleDiff](https://raw.github.com/SimonCropp/ObjectApproval/master/src/SampleDiff.png)


### Validating multiple instances

When validating multiple instances, an [anonymous type](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/anonymous-types) can be used for verification

<!-- snippet: anon -->
```cs
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
```
<sup>[snippet source](/src/Tests/Samples.cs#L63-L83)</sup>
<!-- endsnippet -->

Results in the following:

```graphql
{
  person1: {
    GivenNames: 'John',
    FamilyName: 'Smith'
  },
  person2: {
    GivenNames: 'Marianne',
    FamilyName: 'Aguirre'
  }
}
```


## Serializer settings

`SerializerBuilder` is used to build the Json.net `JsonSerializerSettings`. This is done for every verification run by calling `SerializerBuilder.BuildSettings()`.

All modifications of `SerializerBuilder` behavior is global for all verifications and should be done once at assembly load time.


### Default settings

The default serialization settings are:

<!-- snippet: defaultSerialization -->
```cs
var settings = new JsonSerializerSettings
{
    Formatting = Formatting.Indented,
    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    DefaultValueHandling = DefaultValueHandling.Ignore
};
```
<sup>[snippet source](/src/ObjectApproval/Helpers/SerializerBuilder.cs#L71-L80)</sup>
<!-- endsnippet -->


### Single quotes used

[JsonTextWriter.QuoteChar](https://www.newtonsoft.com/json/help/html/P_Newtonsoft_Json_JsonTextWriter_QuoteChar.htm) is set to single quotes `'`. The reason for this is that it makes approval files cleaner and easier to read and visualize/understand differences

To change this behavior use:

```cs
SerializerBuilder.UseDoubleQuotes = true;
```

### QuoteName is false

[JsonTextWriter.QuoteName](https://www.newtonsoft.com/json/help/html/P_Newtonsoft_Json_JsonTextWriter_QuoteName.htm) is set to false. The reason for this is that it makes approval files cleaner and easier to read and visualize/understand differences

To change this behavior use:

```cs
SerializerBuilder.QuoteNames = true;
```

### Empty collections are ignored

By default empty collections are ignored during verification.

To disable this behavior use:

```cs
SerializerBuilder.IgnoreEmptyCollections = false;
```


### Guids are scrubbed

By default guids are sanitized during verification. This is done by finding each guid and taking a counter based that that specific guid. That counter is then used replace the guid values. This allows for repeatable tests when guid values are changing.

<!-- snippet: guid -->
```cs
var guid = Guid.NewGuid();
var target = new GuidTarget
{
    Guid = guid,
    GuidNullable = guid,
    GuidString = guid.ToString(),
    OtherGuid = Guid.NewGuid(),
};

ObjectApprover.VerifyWithJson(target);
```
<sup>[snippet source](/src/Tests/ObjectApproverTests.cs#L17-L30)</sup>
<!-- endsnippet -->

Results in the following:

```graphql
{
  Guid: 'Guid 1',
  GuidNullable: 'Guid 1',
  GuidString: 'Guid 1',
  OtherGuid: 'Guid 2'
}
```

To disable this behavior use:

```cs
SerializerBuilder.ScrubGuids = false;
```


### Change defaults at the verification level

DateTime, Guid, and empty collection behavior can also be controlled at the verification level: 

<!-- snippet: ChangeDefaultsPerVerification -->
```cs
ObjectApprover.VerifyWithJson(target,
    ignoreEmptyCollections:false,
    scrubGuids:false,
    scrubDateTimes:false);
```
<sup>[snippet source](/src/Tests/Samples.cs#L24-L31)</sup>
<!-- endsnippet -->


### Dates are scrubbed

By default dates (`DateTime` and `DateTimeOffset`) are sanitized during verification. This is done by finding each date and taking a counter based that that specific date. That counter is then used replace the date values. This allows for repeatable tests when date values are changing.

<!-- snippet: Date -->
```cs
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
```
<sup>[snippet source](/src/Tests/ObjectApproverTests.cs#L261-L277)</sup>
<!-- endsnippet -->

Results in the following:

```graphql
{
  DateTime: 'DateTime 1',
  DateTimeNullable: 'DateTime 1',
  DateTimeOffset: 'DateTimeOffset 1',
  DateTimeOffsetNullable: 'DateTimeOffset 1',
  DateTimeString: 'DateTimeOffset 2',
  DateTimeOffsetString: 'DateTimeOffset 2'
}
```

To disable this behavior use:

```cs
SerializerBuilder.ScrubDateTimes = false;
```


### Changing settings globally

To change the serialization settings for all verifications use `SerializerBuilder.ExtraSettings`:

<!-- snippet: ExtraSettings -->
```cs
SerializerBuilder.ExtraSettings =
    jsonSerializerSettings =>
    {
        jsonSerializerSettings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
        jsonSerializerSettings.TypeNameHandling = TypeNameHandling.All;
    };
```
<sup>[snippet source](/src/Tests/Samples.cs#L88-L97)</sup>
<!-- endsnippet -->


### Ignoring a type

To ignore all members that match a certain type:

<!-- snippet: AddIgnore -->
```cs
// Done on static startup
SerializerBuilder.IgnoreMembersWithType<ToIgnore>();

var target = new IgnoreTypeTarget
{
    ToIgnore = new ToIgnore
    {
        Property = "Value"
    }
};

ObjectApprover.VerifyWithJson(target);
```
<sup>[snippet source](/src/Tests/ObjectApproverTests.cs#L36-L51)</sup>
<!-- endsnippet -->


### Ignore member by expressions

To ignore members of a certain type using an expression:

<!-- snippet: IgnoreMemberByExpression -->
```cs
// Done on static startup
SerializerBuilder.IgnoreMember<IgnoreExplicitTarget>(x => x.Property);
SerializerBuilder.IgnoreMember<IgnoreExplicitTarget>(x => x.Field);
SerializerBuilder.IgnoreMember<IgnoreExplicitTarget>(x => x.GetOnlyProperty);
SerializerBuilder.IgnoreMember<IgnoreExplicitTarget>(x => x.PropertyThatThrows);


var target = new IgnoreExplicitTarget
{
    Include = "Value",
    Field = "Value",
    Property = "Value"
};
ObjectApprover.VerifyWithJson(target);
```
<sup>[snippet source](/src/Tests/ObjectApproverTests.cs#L67-L83)</sup>
<!-- endsnippet -->


### Ignore member by name

To ignore members of a certain type using type and name:

<!-- snippet: IgnoreMemberByName -->
```cs
// Done on static startup
var type = typeof(IgnoreExplicitTarget);
SerializerBuilder.IgnoreMember(type, "Property");
SerializerBuilder.IgnoreMember(type, "Field");
SerializerBuilder.IgnoreMember(type, "GetOnlyProperty");
SerializerBuilder.IgnoreMember(type, "PropertyThatThrows");


var target = new IgnoreExplicitTarget
{
    Include = "Value",
    Field = "Value",
    Property = "Value"
};
ObjectApprover.VerifyWithJson(target);
```
<sup>[snippet source](/src/Tests/ObjectApproverTests.cs#L89-L107)</sup>
<!-- endsnippet -->


### Scrubber

A scrubber can be used to cleanup or sanitize the resultant serialized string prior to verification.

<!-- snippet: Scrubber -->
```cs
var target = new ToBeScrubbed
{
    RowVersion = "0x00000000000007D3"
};

ObjectApprover.VerifyWithJson(target,
    scrubber: s => s.Replace("0x00000000000007D3", "ThRowVersion"));
```
<sup>[snippet source](/src/Tests/Samples.cs#L10-L20)</sup>
<!-- endsnippet -->

Results in the following:

```graphql
{
  RowVersion: 'ThRowVersion'
}
```


## Icon

<a href="http://thenounproject.com/term/helmet/9554/" target="_blank">Helmet</a> designed by <a href="http://thenounproject.com/alterego" target="_blank">Leonidas Ikonomou</a> from The Noun Project
