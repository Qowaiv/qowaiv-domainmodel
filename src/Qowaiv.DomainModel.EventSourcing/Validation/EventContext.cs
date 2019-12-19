namespace Qowaiv.DomainModel.EventSourcing.Validation
{
    /// <summary>Event (validation) context.</summary>
    /// <typeparam name="TAggregate">
    /// The type of the aggregate root.
    /// </typeparam>
    /// <typeparam name="TEvent">
    /// The type of the event.
    /// </typeparam>
    /// <remarks>
    /// Used to do the (optional) pre-dispatch validation.
    /// </remarks>
    public sealed class EventContext<TAggregate, TEvent>
        where TAggregate : AggregateRoot<TAggregate>
        where TEvent : class
    {
        /// <summary>Initializes a new instance of the <see cref="EventContext{TAggregate, TEvent}"/> class.</summary>
        /// <param name="aggregate">
        /// The aggregate root.
        /// </param>
        /// <param name="event">
        /// The event.
        /// </param>
        public EventContext(TAggregate aggregate, TEvent @event)
        {
            Aggregate = Guard.NotNull(aggregate, nameof(aggregate));
            Event = Guard.NotNull(@event, nameof(@event));
        }

       /// <summary>Gets the aggregate root.</summary>
        public TAggregate Aggregate { get; }

        /// <summary>Gets the event.</summary>
        public TEvent Event { get; }

        /// <inheritdoc />
        public override string ToString() => $"ID {Aggregate.Id:B}, Version: {Aggregate.Version}, Event: {Event}";
    }
}
