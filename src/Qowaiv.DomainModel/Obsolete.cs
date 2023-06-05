namespace Qowaiv.DomainModel;

/// <summary>Factory method for creating <see cref="AggregateRoot{TAggregate, TId}"/> from stored events.</summary>
[ExcludeFromCodeCoverage/* Justification = To help converting only. */]
[Obsolete("use Aggregate instead.", error: true)]
public static class AggregateRoot
{
    /// <summary>Creates an <see cref="AggregateRoot{TAggregate, TId}"/> from committed events.</summary>
    /// <typeparam name="TAggregate">
    /// The type of aggregate to create.
    /// </typeparam>
    /// <typeparam name="TId">
    /// The type of identifier of the aggregate.
    /// </typeparam>
    /// <param name="buffer">
    /// The buffer to replay.
    /// </param>
    [Pure]
    public static TAggregate FromStorage<TAggregate, TId>(EventBuffer<TId> buffer)
        where TAggregate : AggregateRoot<TAggregate, TId>, new() => throw new NotSupportedException();
}

/// <summary>Represents an (domain-driven design) aggregate root that is based on event sourcing.</summary>
/// <typeparam name="TAggregate">
/// The type of the aggregate root itself.
/// </typeparam>
[ExcludeFromCodeCoverage/* Justification = To help converting only. */]
[Obsolete("use Aggregate<TAggregate> instead.", error: true)]
public abstract class AggregateRoot<TAggregate>
    where TAggregate : AggregateRoot<TAggregate>
{
    /// <summary>Initializes a new instance of the <see cref="AggregateRoot{TAggregate}"/> class.</summary>
    /// <param name="validator">
    /// A custom <paramref name="validator"/> to validate the aggregate.
    /// </param>
    protected AggregateRoot(IValidator<TAggregate> validator) => throw new NotSupportedException();

    /// <summary>The validator that ensures that after applying events the
    /// aggregate is still valid.
    /// </summary>
    protected IValidator<TAggregate> Validator => throw new NotSupportedException();

    /// <summary>Gets an <see cref="ImmutableCollection.Empty"/> collection.</summary>
#pragma warning disable S2743 // Static fields should not be used in generic types
    // FP: nothing is shared, by design.
    protected static ImmutableCollection Events => throw new NotSupportedException();
#pragma warning restore S2743 // Static fields should not be used in generic types

    /// <summary>The dynamic </summary>
    protected virtual EventDispatcher Dispatcher => throw new NotSupportedException();

    /// <summary>Adds the events to the linked event buffer.</summary>
    /// <param name="events">
    /// The events to add to the event buffer.
    /// </param>
    /// <remarks>
    /// This method is only called if after applying the events, the aggregate
    /// is still valid.
    /// </remarks>
    protected abstract void AddEventsToBuffer(IReadOnlyCollection<object> events);

    /// <summary>Clones the current instance.</summary>
    /// <remarks>
    /// It is advised to do this by replaying all previous events.
    /// </remarks>
    [Pure]
    protected abstract TAggregate Clone();

    /// <summary>Applies a single event.</summary>
    [Pure]
    protected Result<TAggregate> ApplyEvent(object @event) => throw new NotSupportedException();

    /// <summary>Applies the events.</summary>
    [Pure]
    protected Result<TAggregate> ApplyEvents(params object[] events) => throw new NotSupportedException();

    /// <summary>Applies the events.</summary>
    [Pure]
    protected Result<TAggregate> Apply(IEnumerable<object> events) => throw new NotSupportedException();

    /// <summary>Allows to pre-process an event before applying it.</summary>
    [Pure]
    protected virtual object PreProcessEvent(object @event) => throw new NotSupportedException();

    /// <summary>Loads the state of the aggregate root by replaying events.</summary>
    protected void Replay(IEnumerable<object> events) => throw new NotSupportedException();

    /// <summary>Root to define guarding conditions on.</summary>
    protected Must<TAggregate> Must => throw new NotSupportedException();
}

/// <summary>Represents an (domain-driven design) aggregate root that is based on event sourcing.</summary>
/// <typeparam name="TAggregate">
/// The type of the aggregate root itself.
/// </typeparam>
/// <typeparam name="TId">
/// The type of the identifier.
/// </typeparam>
[ExcludeFromCodeCoverage/* Justification = To help converting only. */]
[Obsolete("use Aggregate<TAggregate, TId> instead.", error: true)]
public class AggregateRoot<TAggregate, TId> : AggregateRoot<TAggregate>
    where TAggregate : AggregateRoot<TAggregate, TId>, new()
{
    /// <summary>Initializes a new instance of the <see cref="AggregateRoot{TAggregate, TId}"/> class.</summary>
    /// <param name="validator">
    /// A custom <paramref name="validator"/> to validate the aggregate.
    /// </param>
    /// <param name="aggregateId">
    /// The identifier of the aggregate.
    /// </param>
    protected AggregateRoot(TId aggregateId, IValidator<TAggregate> validator) : base(validator) => throw new NotSupportedException();

    /// <summary>Gets the identifier.</summary>
    public TId Id => throw new NotSupportedException();

    /// <summary>Gets the version of aggregate root.</summary>
    public int Version => throw new NotSupportedException();

    /// <summary>Gets the buffer with the recently added events.</summary>
    public EventBuffer<TId> Buffer => throw new NotImplementedException();

    /// <inheritdoc/>
    protected override void AddEventsToBuffer(IReadOnlyCollection<object> events) => throw new NotSupportedException();
    /// <inheritdoc/>
    [Pure]
    protected override TAggregate Clone() => throw new NotSupportedException();
}
