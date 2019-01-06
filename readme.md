
![Icon](https://raw.github.com/SimonCropp/ObjectApproval/master/icon.png)

Extends [ApprovalTests](https://github.com/approvals/ApprovalTests.Net) to allow simple approval of complex models.

**This project is supported by the community via [Patreon sponsorship](https://www.patreon.com/join/simoncropp). If you are using this project to deliver business value or build commercial software it is expected that you will provide support [via Patreon](https://www.patreon.com/join/simoncropp).**


## The NuGet package [![NuGet Status](http://img.shields.io/nuget/v/ObjectApproval.svg?style=flat)](https://www.nuget.org/packages/ObjectApproval/)

https://nuget.org/packages/ObjectApproval/

    PM> Install-Package ObjectApproval


## Usage

Assuming you have previously verified and approved using this. 

```csharp
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

Then you attempt to verify this 

```csharp
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

The serialized json version of these will then be compared and you will be displayed the differences in the diff tool you have asked ApprovalTests to use. For example

![SampleDiff](https://raw.github.com/SimonCropp/ObjectApproval/master/src/SampleDiff.png)


## Icon

<a href="http://thenounproject.com/term/helmet/9554/" target="_blank">Helmet</a> designed by <a href="http://thenounproject.com/alterego" target="_blank">Leonidas Ikonomou</a> from The Noun Project
