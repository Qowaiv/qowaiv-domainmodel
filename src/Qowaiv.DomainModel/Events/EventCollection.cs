using Qowaiv.DomainModel.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Qowaiv.DomainModel.Events
{
    /// <summary>Represents a read-only collection of events.</summary>
    [DebuggerDisplay("Count: {Count}")]
    [DebuggerTypeProxy(typeof(CollectionDebugView))]
    public partial class EventCollection : IReadOnlyCollection<object>
    {
        /// <summary>Gets an empty event collection.</summary>
        public static readonly EventCollection Empty = new();

        /// <summary>Initializes a new instance of the <see cref="EventCollection"/> class.</summary>     
        protected EventCollection() { }

        /// <summary>Gets the total of events in the collection.</summary>
        public int Count => Enumerable.Count(this);

        /// <summary>Adds events to the collection.</summary>
        public EventCollection Add(params object[] events) => Add<object[]>(events);

        /// <summary>Adds an event/events to the collection.</summary>
        /// <param name="event">
        /// The event(s) to add.
        /// </param>
        /// <typeparam name="TEvent">
        /// The type of the event(s).
        /// </typeparam>
        public EventCollection Add<TEvent>(TEvent @event) where TEvent : class
            => @event switch
            {
                null => this,
                string => throw new ArgumentException(QowaivDomainModelMessages.ArgumentException_StringNotAnEvent, nameof(@event)),
                IEnumerable enumerable => new Collection(enumerable, this),
                _ => new Single(@event, this),
            };

        /// <summary>Starts a conditional addition.</summary>
        public If If(bool? condition) => If(condition == true);

        /// <summary>Starts a conditional addition.</summary>
        public If If(bool condition) => new(condition, this);

        /// <inheritdoc />
        public IEnumerator<object> GetEnumerator()
            => Enumerate().GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>Enumerates through all events.</summary>
        protected virtual IEnumerable<object> Enumerate() => Enumerable.Empty<object>();
    }
}
