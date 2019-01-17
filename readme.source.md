
![Icon](https://raw.github.com/SimonCropp/ObjectApproval/master/icon.png)

Extends [ApprovalTests](https://github.com/approvals/ApprovalTests.Net) to allow simple approval of complex models.

**This project is supported by the community via [Patreon sponsorship](https://www.patreon.com/join/simoncropp). If you are using this project to deliver business value or build commercial software it is expected that you will provide support [via Patreon](https://www.patreon.com/join/simoncropp).**


## The NuGet package [![NuGet Status](http://img.shields.io/nuget/v/ObjectApproval.svg?style=flat)](https://www.nuget.org/packages/ObjectApproval/)

https://nuget.org/packages/ObjectApproval/

    PM> Install-Package ObjectApproval


## Usage

Assuming you have previously verified and approved using this. 

snippet: before

Then you attempt to verify this 

snippet: after

The serialized json version of these will then be compared and you will be displayed the differences in the diff tool you have asked ApprovalTests to use. For example:

![SampleDiff](https://raw.github.com/SimonCropp/ObjectApproval/master/src/SampleDiff.png)


## Serializer settings

`SerializerBuilder` is used to build the Json.net `JsonSerializerSettings`. This is done for every verification run by calling `SerializerBuilder.BuildSettings()`.

All modifications of `SerializerBuilder` behavior is global for all verifications and should be done once at assembly load time.


### Default settings

The default serialization settings are:

snippet: defaultSerialization


### Empty collections are ignored

By default empty collections are ignored during verification.

To disable this behavior use:

```cs
SerializerBuilder.IgnoreEmptyCollections = false;
```


### Guids are scrubbed

By default guids are sanitized during verification. This is done by finding each guid and taking a counter based that that specific guid. That counter is then used replace the guid values. This allows for repeatable tests when guid values are changing.

snippet: guid

```json
{
  "Guid": "Guid 1",
  "GuidNullable": "Guid 1",
  "GuidString": "Guid 1",
  "OtherGuid": "Guid 2"
}
```

To disable this behavior use:

```cs
SerializerBuilder.ScrubGuids = false;
```


### Change defaults at the verification level

DateTime, Guid, and empty collection behavior can also be controlled at the verification level: 

snippet: ChangeDefaultsPerVerification


### Dates are scrubbed

By default dates (`DateTime` and `DateTimeOffset`) are sanitized during verification. This is done by finding each date and taking a counter based that that specific date. That counter is then used replace the date values. This allows for repeatable tests when date values are changing.

snippet: Date

```json
{
  "DateTime": "DateTime 1",
  "DateTimeNullable": "DateTime 1",
  "DateTimeOffset": "DateTimeOffset 1",
  "DateTimeOffsetNullable": "DateTimeOffset 1",
  "DateTimeString": "DateTimeOffset 2",
  "DateTimeOffsetString": "DateTimeOffset 2"
}
```

To disable this behavior use:

```cs
SerializerBuilder.ScrubDateTimes = false;
```




### Changing settings globally

To change the serialization settings for all verifications use `SerializerBuilder.ExtraSettings`:

snippet: ExtraSettings


### Ignoring a type

To ignore all members that match a certain type:

snippet: AddIgnore


### Ignore member by expressions

To ignore members of a certain type using an expression:

snippet: IgnoreMemberByExpression


### Ignore member by name

To ignore members of a certain type using type and name:

snippet: IgnoreMemberByName


### Scrubber

snippet: Scrubber

```json
{
  "RowVersion": "ThRowVersion"
}
```


## Icon

<a href="http://thenounproject.com/term/helmet/9554/" target="_blank">Helmet</a> designed by <a href="http://thenounproject.com/alterego" target="_blank">Leonidas Ikonomou</a> from The Noun Project
