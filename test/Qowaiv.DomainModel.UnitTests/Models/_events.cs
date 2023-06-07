namespace Qowaiv.DomainModel.UnitTests.Models;

[EmptyTestClass]
internal record EmptyEvent();

internal record StoredEvent(object Id, int Version, object Payload);
