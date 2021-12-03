namespace Qowaiv.DomainModel;

/// <summary>A function to convert the event (payload) from the stored event.</summary>
/// <typeparam name="TId">
/// The type of the identifier of the aggregate.
/// </typeparam>
/// <typeparam name="TStoredEvent">
/// The Type of the stored event.
/// </typeparam>
/// <param name="aggregateId">
/// The identifier of the aggregate.
/// </param>
/// <param name="version">
/// The version of the event.
/// </param>
/// <param name="event">
/// The event (payload).
/// </param>
/// <returns>
/// The converted event.
/// </returns>
[Pure]
public delegate TStoredEvent ConvertToStoredEvent<in TId, out TStoredEvent>(TId aggregateId, int version, object @event);

/// <summary>A buffer of events that should be added to an event stream.</summary>
/// <typeparam name="TId">
/// The type of the identifier of the aggregate.
/// </typeparam>
[DebuggerDisplay("{DebuggerDisplay}")]
[DebuggerTypeProxy(typeof(CollectionDebugView))]
public class EventBuffer<TId> : IEnumerable<object>
{
    private readonly ImmutableCollection buffer;
    private readonly int offset;

    /// <summary>Initializes a new instance of the <see cref="EventBuffer{TId}"/> class.</summary>
    /// <param name="aggregateId">
    /// The identifier of the aggregate root.
    /// </param>
    [Obsolete("Use EventBuffer.Empty(aggregateId, version) indeed. This constructor will dropped.")]
    public EventBuffer(TId aggregateId) : this(aggregateId, 0) { }

    /// <summary>Initializes a new instance of the <see cref="EventBuffer{TId}"/> class.</summary>
    /// <param name="aggregateId">
    /// The identifier of the aggregate root.
    /// </param>
    /// <param name="version">
    /// The initial version (offset).
    /// </param>
    [Obsolete("Use EventBuffer.Empty(aggregateId, version) indeed. This constructor will dropped.")]
    public EventBuffer(TId aggregateId, int version)
        : this(aggregateId, version, committed: version, buffer: ImmutableCollection.Empty) { }

    internal EventBuffer(TId aggregateId, int offset, int committed, ImmutableCollection buffer)
    {
        AggregateId = aggregateId;
        CommittedVersion = committed;
        this.offset = offset;
        this.buffer = buffer;
    }

    /// <summary>Gets the identifier of the aggregate root.</summary>
    public TId AggregateId { get; }

    /// <summary>The version of the event buffer.</summary>
    public int Version => buffer.Count + offset;

    /// <summary>Gets the committed version of the event buffer.</summary>
    public int CommittedVersion { get; }

    /// <summary>Get all committed events in the event buffer.</summary>
    public IEnumerable<object> Committed => buffer.Take(CommittedVersion - offset);

    /// <summary>Get all uncommitted events in the event buffer.</summary>
    public IEnumerable<object> Uncommitted => buffer.Skip(CommittedVersion - offset);

    /// <summary>Returns true if the event buffer contains at least one uncommitted event.</summary>
    public bool HasUncommitted => Version != CommittedVersion;

    /// <summary>Returns true if the buffer contains no events.</summary>
    public bool IsEmpty => buffer.Count == 0;

    /// <summary>Adds an event/events to the event buffer.</summary>
    /// <param name="event">
    /// The event(s) to add.
    /// </param>
    /// <remarks>
    /// Null, and null items are ignored.
    /// </remarks>
    [Pure]
    public EventBuffer<TId> Add(object @event)
        => new(AggregateId, offset, CommittedVersion, buffer.Add<object>(@event));

    /// <summary>Adds events to the event buffer.</summary>
    /// <param name="events">
    /// The events to add.
    /// </param>
    [Obsolete("Use Add(event) instead.")]
    public EventBuffer<TId> AddRange(IEnumerable<object> events)
        => new(AggregateId, offset, CommittedVersion, buffer.Add(events));

    /// <summary>Marks all events as being committed.</summary>
    [Pure]
    public EventBuffer<TId> MarkAllAsCommitted()
        => new(AggregateId, offset, Version, buffer);

    /// <summary>Removes the committed events from the buffer.</summary>
    [Obsolete("Do not use, it might break replay capabilities. Functionality will be dropped.")]
    public EventBuffer<TId> ClearCommitted()
        => new(
            aggregateId: AggregateId,
            offset: CommittedVersion,
            committed: CommittedVersion,
            buffer: ImmutableCollection.Empty.Add(buffer.Skip(CommittedVersion - offset)));

    /// <summary>Selects the uncommitted events.</summary>
    /// <typeparam name="TStoredEvent">
    /// The type to convert it to.
    /// </typeparam>
    /// <param name="convert">
    /// The function to convert a <typeparamref name="TStoredEvent"/> from
    /// the aggregate identifier and the version of the event.
    /// </param>
    /// <returns>
    /// The uncommitted events as <typeparamref name="TStoredEvent"/>.
    /// </returns>
    [Pure]
    public IEnumerable<TStoredEvent> SelectUncommitted<TStoredEvent>(ConvertToStoredEvent<TId, TStoredEvent> convert)
    {
        Guard.NotNull(convert, nameof(convert));
        return Uncommitted.Select((@event, index) => convert(AggregateId, CommittedVersion + index + 1, @event));
    }

    /// <inheritdoc/>
    [Pure]
    public IEnumerator<object> GetEnumerator() => buffer.GetEnumerator();

    /// <inheritdoc/>
    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>Returns a <see cref="string"/> that represents the event buffer for debug purposes.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal string DebuggerDisplay
        => Version == CommittedVersion
        ? $"Version: {Version}, Aggregate: {AggregateId}"
        : $"Version: {Version} (Committed: {CommittedVersion}), Aggregate: {AggregateId}";

    /// <summary>Creates an event buffer from some storage.</summary>
    /// <typeparam name="TStoredEvent">
    /// The type of the stored event.
    /// </typeparam>
    /// <param name="aggregateId">
    /// The identifier of the aggregate root.
    /// </param>
    /// <param name="storedEvents">
    /// The stored events.
    /// </param>
    /// <param name="convert">
    /// The function that select a 'clean' event from a stored event.
    /// </param>
    /// <returns>
    /// An event buffer with contains only committed events.
    /// </returns>
    [Obsolete("Use EventBuffer.FromStorage() indeed. This method will be dropped.")]
    public static EventBuffer<TId> FromStorage<TStoredEvent>(
        TId aggregateId,
        IEnumerable<TStoredEvent> storedEvents,
        ConvertFromStoredEvent<TStoredEvent> convert)
        => EventBuffer.FromStorage(aggregateId, 0, storedEvents, convert);

    /// <summary>Creates an event buffer from some storage.</summary>
    /// <typeparam name="TStoredEvent">
    /// The type of the stored event.
    /// </typeparam>
    /// <param name="aggregateId">
    /// The identifier of the aggregate root.
    /// </param>
    /// <param name="initialVersion">
    /// The initial version (offset).
    /// </param>
    /// <param name="storedEvents">
    /// The stored events.
    /// </param>
    /// <param name="convert">
    /// The function that select a 'clean' event from a stored event.
    /// </param>
    /// <returns>
    /// An event buffer with contains only committed events.
    /// </returns>
    [Obsolete("Use EventBuffer.FromStorage() indeed. This method will be dropped.")]
    public static EventBuffer<TId> FromStorage<TStoredEvent>(
        TId aggregateId,
        int initialVersion,
        IEnumerable<TStoredEvent> storedEvents,
        ConvertFromStoredEvent<TStoredEvent> convert)
        => EventBuffer.FromStorage(aggregateId, initialVersion, storedEvents, convert);
}
