namespace Qowaiv.DomainModel;

/// <summary>Represents an (domain-driven design) aggregate root that is based on event sourcing.</summary>
/// <typeparam name="TAggregate">
/// The type of the aggregate root itself.
/// </typeparam>
public abstract class AggregateRoot<TAggregate>
    where TAggregate : AggregateRoot<TAggregate>
{
    /// <summary>Initializes a new instance of the <see cref="AggregateRoot{TAggregate}"/> class.</summary>
    /// <param name="validator">
    /// A custom <paramref name="validator"/> to validate the aggregate.
    /// </param>
    protected AggregateRoot(IValidator<TAggregate> validator)
    {
        Validator = Guard.NotNull(validator, nameof(validator));
        Dispatcher = new ExpressionCompilingEventDispatcher<TAggregate>((TAggregate)this);
    }

    /// <summary>The validator that ensures that after applying events the
    /// aggregate is still valid.
    /// </summary>
    protected IValidator<TAggregate> Validator { get; }

    /// <summary>Gets an <see cref="ImmutableCollection.Empty"/> collection.</summary>
#pragma warning disable S2743 // Static fields should not be used in generic types
    // FP: nothing is shared, by design.
    protected static ImmutableCollection Events => ImmutableCollection.Empty;
#pragma warning restore S2743 // Static fields should not be used in generic types

    /// <summary>The dynamic </summary>
    protected virtual EventDispatcher Dispatcher { get; }

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
    protected Result<TAggregate> ApplyEvent(object @event)
        => Apply(new Singleton(Guard.NotNull(@event, nameof(@event))));

    /// <summary>Applies the events.</summary>
    [Pure]
    protected Result<TAggregate> ApplyEvents(params object[] events)
        => Apply(events);

    /// <summary>Applies the events.</summary>
    [Pure]
    protected Result<TAggregate> Apply(IEnumerable<object> events)
    {
        Guard.NotNull(events, nameof(events));

        var updated = Clone();
        var append = AppendOnlyCollection.Empty.Add(events.Select(PreProcessEvent));
        updated.Replay(append);

        var result = updated.Validator.Validate(updated);
        if (result.IsValid)
        {
            updated.AddEventsToBuffer(append);
        }
        return result;
    }

    /// <summary>Allows to pre-process an event before applying it.</summary>
    [Pure]
    protected virtual object PreProcessEvent(object @event) => @event;

    /// <summary>Loads the state of the aggregate root by replaying events.</summary>
    protected void Replay(IEnumerable<object> events)
    {
        foreach (var @event in Guard.NotNull(events, nameof(events)))
        {
            Dispatcher.When(@event);
        }
    }

    /// <summary>Root to define guarding conditions on.</summary>
    protected Must<TAggregate> Must => ((TAggregate)this).Must();
}
