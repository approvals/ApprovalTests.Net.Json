
![Icon](https://raw.github.com/SimonCropp/ObjectApproval/master/icon.png)

Extends [ApprovalTests](https://github.com/approvals/ApprovalTests.Net) to allow simple approval of complex models.

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
<sup>[snippet source](/src/Tests/Samples.cs#L8-L24)</sup>
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
<sup>[snippet source](/src/Tests/Samples.cs#L43-L60)</sup>
<!-- endsnippet -->

The serialized json version of these will then be compared and you will be displayed the differences in the diff tool you have asked ApprovalTests to use. For example

![SampleDiff](https://raw.github.com/SimonCropp/ObjectApproval/master/src/SampleDiff.png)


## Serializer settings

`SerializerBuilder` is used to build the Json.net `JsonSerializerSettings`. This is done for every verification run by calling `SerializerBuilder.BuildSettings()`.

All modifications of `SerializerBuilder` behavior is global for all verifications and should be done once at assembly load time.

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
<sup>[snippet source](/src/Tests/Samples.cs#L29-L38)</sup>
<!-- endsnippet -->


### Ignoring a type

To ignore all members that match a certain type:

<!-- snippet: AddIgnore -->
```cs
var target = new IgnoreTypeTarget
{
    ToIgnore = new ToIgnore
    {
        Property = "Value"
    }
};
SerializerBuilder.AddIgnore<ToIgnore>();

ObjectApprover.VerifyWithJson(target);
```
<sup>[snippet source](/src/Tests/ObjectApproverTests.cs#L31-L44)</sup>
<!-- endsnippet -->


### Ignore member by expressions

To ignore members of a certain type using an expression:

<!-- snippet: IgnoreMemberByExpression -->
```cs
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
```
<sup>[snippet source](/src/Tests/ObjectApproverTests.cs#L60-L72)</sup>
<!-- endsnippet -->


### Ignore member by name

To ignore members of a certain type using type and name:

<!-- snippet: IgnoreMemberByName -->
```cs
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
```
<sup>[snippet source](/src/Tests/ObjectApproverTests.cs#L78-L93)</sup>
<!-- endsnippet -->



## Icon

<a href="http://thenounproject.com/term/helmet/9554/" target="_blank">Helmet</a> designed by <a href="http://thenounproject.com/alterego" target="_blank">Leonidas Ikonomou</a> from The Noun Project
