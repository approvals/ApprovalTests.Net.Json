
![Icon](https://raw.github.com/SimonCropp/ObjectApproval/master/Icons/package_icon.png)

Extends [ApprovalTests](https://github.com/approvals/ApprovalTests.Net) to allow simple approval of complex models.

## The nuget package  [![NuGet Status](http://img.shields.io/nuget/v/ObjectApproval.svg?style=flat)](https://www.nuget.org/packages/ObjectApproval/)

https://nuget.org/packages/ObjectApproval/

    PM> Install-Package ObjectApproval

## Usage

Assuming you have previously verified and approved using this. 

```
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

```
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

![SampleDiff](https://raw.github.com/SimonCropp/ObjectApproval/master/SampleDiff.png)

## Icon 

<a href="http://thenounproject.com/term/helmet/9554/" target="_blank">Helmet</a> designed by <a href="http://thenounproject.com/alterego" target="_blank">Leonidas Ikonomou</a> from The Noun Project
