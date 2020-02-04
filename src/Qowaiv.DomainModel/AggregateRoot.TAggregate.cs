using Qowaiv.Validation.Abstractions;
using System;
using System.Linq;

namespace Qowaiv.DomainModel
{
    /// <summary>Represents an (domain-driven design) aggregate root that is based on event sourcing.</summary>
    /// <typeparam name="TAggregate">
    /// The type of the aggregate root itself.
    /// </typeparam>
    public abstract class AggregateRoot<TAggregate> : ValidatableAggregate<TAggregate>
        where TAggregate : AggregateRoot<TAggregate>
    {
        /// <summary>Initializes a new instance of the <see cref="AggregateRoot{TAggregate}"/> class.</summary>
        /// <param name="validator">
        /// A custom validator.
        /// </param>
        protected AggregateRoot(IValidator<TAggregate> validator) : this(Guid.NewGuid(), validator) { }

        /// <summary>Initializes a new instance of the <see cref="AggregateRoot{TAggregate}"/> class.</summary>
        /// <param name="id">
        /// The identifier of the aggregate root.
        /// </param>
        /// <param name="validator">
        /// A custom validator.
        /// </param>
        protected AggregateRoot(Guid id, IValidator<TAggregate> validator) : base(validator)
        {
            EventStream = new EventStream(id);
        }

        /// <summary>The identifier of the entity.</summary>
        public Guid Id => EventStream.AggregateId;

        /// <summary>Gets the event stream representing the state of the aggregate root.</summary>
        public EventStream EventStream { get; private set; }

        /// <summary>Gets the version of the aggregate root.</summary>
        public int Version => EventStream.Version;

        /// <inheritdoc/>
        protected override void AddEventsToStream(params object[] events)
        {
            EventStream.AddRange(events);
        }

        /// <summary>Loads the state of the aggregate root based on historical events.</summary>
        internal void LoadEvents(EventStream stream)
        {
            EventStream = new EventStream(stream.AggregateId, stream.Version);

            lock (stream.Lock())
            {
                Replay(stream.Select(message => message.Event));
            }
        }
    }
}
