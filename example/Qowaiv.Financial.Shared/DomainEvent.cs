using System;
using Qowaiv;
using System.Collections.Generic;
using System.Text;

namespace Qowaiv.Financial.Shared
{
    /// <summary>Represents a domain event.</summary>
    public class DomainEvent
    {
        /// <summary>Creates a new instance of the <see cref="DomainEvent"/> class.</summary>
        public DomainEvent(object aggregateId, object @event)
        {
            AggregateId = Guard.NotNull(aggregateId, nameof(aggregateId));
            Event = Guard.NotNull(@event, nameof(@event));
        }

        public static object Clock { get; private set; }

        /// <summary>The aggregate identifier of the event.</summary>
        public object AggregateId { get; }

        /// <summary>The <see cref="DateTime"/> on which the event occurred (UTC based).</summary>
        public DateTime OccurredUtc { get; }// = Clock.UtcNow();

        /// <summary>Gets the event type.</summary>
        public virtual Type EventType => Event.GetType();

        /// <summary>Gets the event data.</summary>
        public object Event { get; }
    }
}
