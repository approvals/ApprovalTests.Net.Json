# <img src="/src/icon.png" height="30px"> ApprovalTests.Net.Json

[![Build status](https://ci.appveyor.com/api/projects/status/tsx7biy2rgwtd3wn/branch/master?svg=true)](https://ci.appveyor.com/project/SimonCropp/ApprovalTests.Net.Json)
[![NuGet Status](https://img.shields.io/nuget/v/ApprovalTests.Net.Json.svg?cacheSeconds=86400)](https://www.nuget.org/packages/ApprovalTests.Net.Json/)

Extends [ApprovalTests](https://github.com/approvals/ApprovalTests.Net) to allow simple approval of complex models using [Json.net](https://www.newtonsoft.com/json).

toc


## NuGet package

https://nuget.org/packages/ApprovalTests.Net.Json/


## Usage

Assuming you have previously verified and approved using this.

snippet: before

Then you attempt to verify this 

snippet: after

The serialized json version of these will then be compared and you will be displayed the differences in the diff tool you have asked ApprovalTests to use. For example:

![SampleDiff](/src/SampleDiff.png)

Note that the output is technically not valid json. [Single quotes are used](#single-quotes-used) and [names are not quoted](#quotename-is-false). The reason for this is to make the resulting output easier to read and understand.


### Validating multiple instances

When validating multiple instances, an [anonymous type](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/anonymous-types) can be used for verification

snippet: anon

Results in the following:

snippet: Samples.Anon.approved.txt


## Serializer settings

`SerializerBuilder` is used to build the Json.net `JsonSerializerSettings`. This is done for every verification run by calling `SerializerBuilder.BuildSettings()`.

All modifications of `SerializerBuilder` behavior is global for all verifications and should be done once at assembly load time.


### Default settings

The default serialization settings are:

snippet: defaultSerialization


### Single quotes used

[JsonTextWriter.QuoteChar](https://www.newtonsoft.com/json/help/html/P_Newtonsoft_Json_JsonTextWriter_QuoteChar.htm) is set to single quotes `'`. The reason for this is that it makes approval files cleaner and easier to read and visualize/understand differences


### QuoteName is false

[JsonTextWriter.QuoteName](https://www.newtonsoft.com/json/help/html/P_Newtonsoft_Json_JsonTextWriter_QuoteName.htm) is set to false. The reason for this is that it makes approval files cleaner and easier to read and visualize/understand differences


### Empty collections are ignored

By default empty collections are ignored during verification.

To disable this behavior use:

```cs
SerializerBuilder.IgnoreEmptyCollections = false;
```


### Guids are scrubbed

By default guids are sanitized during verification. This is done by finding each guid and taking a counter based that that specific guid. That counter is then used replace the guid values. This allows for repeatable tests when guid values are changing.

snippet: guid

Results in the following:

snippet: ObjectApproverTests.ShouldReUseGuid.approved.txt

To disable this behavior use:

```cs
SerializerBuilder.ScrubGuids = false;
```


### Dates are scrubbed

By default dates (`DateTime` and `DateTimeOffset`) are sanitized during verification. This is done by finding each date and taking a counter based that that specific date. That counter is then used replace the date values. This allows for repeatable tests when date values are changing.

snippet: Date

Results in the following:

snippet: ObjectApproverTests.ShouldReUseDatetime.approved.txt

To disable this behavior use:

```cs
SerializerBuilder.ScrubDateTimes = false;
```


### Default Booleans are ignored

By default values of `bool` and `bool?` are ignored during verification. So properties that equate to 'false' will not be written,

To disable this behavior use:

```cs
SerializerBuilder.IgnoreFalse = false;
```


### Change defaults at the verification level

`DateTime`, `DateTimeOffset`, `Guid`, `bool`, and empty collection behavior can also be controlled at the verification level: 

snippet: ChangeDefaultsPerVerification


### Changing settings globally

To change the serialization settings for all verifications use `SerializerBuilder.ExtraSettings`:

snippet: ExtraSettings


### Scoped settings

snippet: ScopedSerializer

Result:

snippet: Samples.ScopedSerializer.approved.txt


### Ignoring a type

To ignore all members that match a certain type:

snippet: AddIgnoreType

Result:

snippet: ObjectApproverTests.IgnoreType.approved.txt


### Ignoring a instance

To ignore instances of a type based on delegate:

snippet: AddIgnoreInstance

Result:

snippet: ObjectApproverTests.IgnoreInstance.approved.txt


### Ignore member by expressions

To ignore members of a certain type using an expression:

snippet: IgnoreMemberByExpression

Result:

snippet: ObjectApproverTests.IgnoreMemberByExpression.approved.txt


### Ignore member by name

To ignore members of a certain type using type and name:

snippet: IgnoreMemberByName

Result:

snippet: ObjectApproverTests.IgnoreMemberByName.approved.txt


### Members that throw

Members that throw exceptions can be excluded from serialization based on the exception type or properties.

By default members that throw `NotImplementedException` or `NotSupportedException` are ignored.

Note that this is global for all members on all types.

Ignore by exception type:

snippet: IgnoreMembersThatThrow

Result:

snippet: ObjectApproverTests.CustomExceptionProp.approved.txt

Ignore by exception type and expression:

snippet: IgnoreMembersThatThrowExpression

Result:

snippet: ObjectApproverTests.ExceptionMessageProp.approved.txt


### Scrubber

A scrubber can be used to cleanup or sanitize the resultant serialized string prior to verification.

snippet: Scrubber

Results in the following:

```graphql
{
  RowVersion: 'ThRowVersion'
}
```

## Named Tuples

Instances of [named tuples](https://docs.microsoft.com/en-us/dotnet/csharp/tuples#named-and-unnamed-tuples) can be verified using `ObjectApprover.VerifyTuple`.

Due to the use of [ITuple](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.ituple), this approach is only available an net472+ and netcoreapp2.2+.

Given a method that returns a named tuple:

snippet: MethodWithNamedTuple

Can be verified:

snippet: VerifyTuple

Resulting in:

snippet: ObjectApproverTests.NamedTuple.approved.txt


## Release Notes

See [closed milestones](../../milestones?state=closed).


## Icon

[Helmet](https://thenounproject.com/term/helmet/9554/) designed by [Leonidas Ikonomou](https://thenounproject.com/alterego) from [The Noun Project](https://thenounproject.com).