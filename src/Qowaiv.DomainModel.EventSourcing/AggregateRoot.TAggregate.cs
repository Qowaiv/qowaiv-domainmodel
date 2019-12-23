using Qowaiv.DomainModel.EventSourcing.Dynamic;
using Qowaiv.DomainModel.EventSourcing.Validation;
using Qowaiv.Validation.Abstractions;
using System;
using System.Diagnostics;

namespace Qowaiv.DomainModel.EventSourcing
{
    /// <summary>Represents an (domain-driven design) aggregate root that is based on event sourcing.</summary>
    /// <typeparam name="TAggregate">
    /// The type of the aggregate root itself.
    /// </typeparam>
    public abstract class AggregateRoot<TAggregate>
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
        protected AggregateRoot(Guid id, IValidator<TAggregate> validator)
        {
            EventStream = new EventStream(id);
            this.validator = Guard.NotNull(validator, nameof(validator));
        }

        /// <summary>The identifier of the entity.</summary>
        public Guid Id => EventStream.AggregateId;

        private readonly IValidator<TAggregate> validator;

        /// <summary>Gets the event stream representing the state of the aggregate root.</summary>
        public EventStream EventStream { get; private set; }

        /// <summary>Gets the version of the aggregate root.</summary>
        public int Version => EventStream.Version;

        /// <summary>Applies an event.</summary>
        protected Result<TAggregate> ApplyEvent(object @event) => ApplyEvents(@event);

        /// <summary>Applies the events.</summary>
        protected Result<TAggregate> ApplyEvents(params object[] events)
        {
            Guard.HasAny(events, nameof(events));

            var result = Result.For((TAggregate)this);

            lock (EventStream.Lock())
            {
                foreach (var @event in events)
                {
                    result = result
                        .Act(m => validator.ValidateEvent(m, @event))
                        .Act(m => Apply(m, @event));
                }

                result = result.Act(m => validator.Validate(m));

                if (result.IsValid)
                {
                    EventStream.AddRange(events);
                }
                return result;
            }
        }

        private static Result Apply(TAggregate model, object @event)
        {
            model.AsDynamic().Apply(@event);
            return Result.OK;
        }

        /// <summary>Loads the state of the aggregate root based on historical events.</summary>
        internal void LoadEvents(EventStream stream)
        {
            EventStream = new EventStream(stream.AggregateId, stream.Version);

            lock (stream.Lock())
            {
                var self = AsDynamic();

                foreach (var e in stream)
                {
                    self.Apply(e.Event);
                }
            }
        }

        /// <summary>Represents the aggregate root as a dynamic.</summary>
        /// <remarks>
        /// By default, this dynamic is only capable of invoking Apply(@event).
        /// If more is wanted, this method should be overridden.
        /// </remarks>
        private dynamic AsDynamic()
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
