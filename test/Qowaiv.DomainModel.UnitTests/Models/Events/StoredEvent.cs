namespace Models.Events;

public sealed record StoredEvent(object Id, int Version, object Payload);
