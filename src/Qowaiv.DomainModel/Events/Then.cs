using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Qowaiv.DomainModel.Events
{
    /// <summary>Represents the conditional addition of an event, after the if-statement.</summary>
    public class Then : EventCollection
    {
        /// <summary>Initializes a new instance of the <see cref="Then"/> class.</summary>
        internal Then(bool done, EventCollection predecessor)
        {
            Done = done;
            Predecessor = predecessor;
        }

        /// <summary>The predecessor <see cref="EventCollection"/>.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private EventCollection Predecessor { get; }

        /// <summary>The predecessor <see cref="EventCollection"/>.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool Done { get; }

        /// <summary>Adds else-if condition.</summary>
        public If ElseIf(bool condition)
            => Done
            ? new If(IfState.Done, Predecessor)
            : new If(condition, Predecessor);

        /// <summary>Adds an (optional) else addition.</summary>
        /// <param name="event">
        /// The event to add.
        /// </param>
        /// <typeparam name="TElseEvent">
        /// The type of the event to add.
        /// </typeparam>
        public EventCollection Else<TElseEvent>(Func<TElseEvent> @event) where TElseEvent : class
            => Done
            ? Predecessor
            : Predecessor.Add(@event());

        /// <inheritdoc />
        internal override IEnumerable<object> Enumerate()
            => Predecessor.Enumerate();
    }
}
