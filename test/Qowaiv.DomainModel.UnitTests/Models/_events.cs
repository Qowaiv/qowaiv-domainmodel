namespace Qowaiv.DomainModel.UnitTests.Models;

[EmptyTestClass]
internal record EmptyEvent();

internal record StoredEvent(object Id, int Version, object Payload);

﻿namespace Qowaiv.DomainModel.UnitTests.Models;

public class NameUpdated
{
    public string? Name { get; init; }
}

public class DateOfBirthUpdated
{
    public Date DateOfBirth { get; init; }
}

[EmptyTestClass]
public class InvalidEvent { }


[EmptyTestClass]
public class SimpleInitEvent { }
