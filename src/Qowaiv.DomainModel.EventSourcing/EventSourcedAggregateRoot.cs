using Qowaiv.DomainModel.EventSourcing.Dynamic;
using Qowaiv.Validation.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Qowaiv.DomainModel.EventSourcing
{
    /// <summary>Represents an (domain-driven design) aggregate root that is based on event sourcing.</summary>
    /// <typeparam name="TAggregate">
    /// The type of the aggregate root itself.
    /// </typeparam>
    public abstract class EventSourcedAggregateRoot<TAggregate> : AggregateRoot<TAggregate>
        where TAggregate : EventSourcedAggregateRoot<TAggregate>
    {
        /// <summary>Initializes a new instance of the <see cref="EventSourcedAggregateRoot{TAggregate}"/> class.</summary>
        /// <param name="validator">
        /// A custom validator.
        /// </param>
        protected EventSourcedAggregateRoot(IValidator<TAggregate> validator) : this(Guid.NewGuid(), validator) { }

        /// <summary>Initializes a new instance of the <see cref="EventSourcedAggregateRoot{TAggregate}"/> class.</summary>
        /// <param name="id">
        /// The identifier of the aggregate root.
        /// </param>
        /// <param name="validator">
        /// A custom validator.
        /// </param>
        protected EventSourcedAggregateRoot(Guid id, IValidator<TAggregate> validator) : base(id, validator)
        {
            EventStream = new EventStream(id);
        }

        /// <inheritdoc />
        public sealed override Guid Id => EventStream.AggregateId;

        /// <summary>Gets the event stream representing the state of the aggregate root.</summary>
        public EventStream EventStream { get; private set; }

        /// <summary>Gets the version of the aggregate root.</summary>
        public int Version => EventStream.Version;

        /// <summary>Applies an event.</summary>
        protected Result<TAggregate> ApplyEvent(object @event)
        {
            Guard.NotNull(@event, nameof(@event));

            lock (EventStream.Lock())
            {
                var result = Update((self) =>
                {
                    self.AsDynamic().Apply(@event);
                });
                if (result.IsValid)
                {
                    EventStream.Add(@event);
                }
                return result;
            }
        }

        /// <summary>Applies the events.</summary>
        protected Result<TAggregate> ApplyEvents(params object[] events) => ApplyEvents(events?.AsEnumerable());

        /// <summary>Applies the events.</summary>
        protected Result<TAggregate> ApplyEvents(IEnumerable<object> events)
        {
            var all = Guard.NotNull(events, nameof(events)).ToArray();

            lock (EventStream.Lock())
            {
                var result = Update((self) =>
                {
                    foreach (var @event in all)
                    {
                        self.AsDynamic().Apply(@event);
                    }
                });
                if (result.IsValid)
                {
                    EventStream.AddRange(all);
                }

                return result;
            }
        }

        /// <summary>Loads the state of the aggregate root based on historical events.</summary>
        internal void LoadEvents(EventStream stream)
        {
            Tracker.Intialize();
            EventStream = new EventStream(stream.AggregateId, stream.Version);

            foreach (var e in stream)
            {
                AsDynamic().Apply(e.Event);
            }
            Tracker.NoBuffering();
        }

        /// <summary>Represents the aggregate root as a dynamic.</summary>
        /// <remarks>
        /// By default, this dynamic is only capable of invoking Apply(@event).
        /// If more is wanted, this method should be overridden.
        /// </remarks>
        protected virtual dynamic AsDynamic()
        {
            if (@dynamic is null)
            {
                @dynamic = new DynamicEventDispatcher(this);
            }
            return @dynamic;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private dynamic @dynamic;
    }
}
