using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Qowaiv.DomainModel.Events
{
    /// <summary>Represents the conditional addition of an event, after the if-statement.</summary>
    /// <typeparam name="TEvent">
    /// The type of the event to add.
    /// </typeparam>
    public class Then<TEvent> : EventCollection where TEvent : class
    {
        /// <summary>Initializes a new instance of the <see cref="Then{TEvent}"/> class.</summary>
        internal Then(If predecessor, Func<TEvent> @event)
        {
            Predecessor = predecessor;
            Event = @event;
        }

        /// <summary>The predecessor <see cref="EventCollection"/>.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private If Predecessor { get; }

        /// <summary>Event factory placeholder.</summary>
        private Func<TEvent> Event { get; }

        /// <summary>Adds an (optional) else addition.</summary>
        /// <param name="event">
        /// The event to add.
        /// </param>
        /// <typeparam name="TElseEvent">
        /// The type of the event to add.
        /// </typeparam>
        public EventCollection Else<TElseEvent>(Func<TElseEvent> @event) where TElseEvent : class
            => Predecessor
            ? Predecessor.Add(Event())
            : Predecessor.Add(@event());

        /// <inheritdoc />
        protected override IEnumerable<object> Enumerate()
            => Predecessor
            ? base.Enumerate().Append(Event())
            : base.Enumerate();
    }
}
