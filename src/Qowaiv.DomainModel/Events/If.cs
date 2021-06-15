using System;
using System.Diagnostics;

namespace Qowaiv.DomainModel.Events
{
    /// <summary>Represents the start of an (logical) if-statement.</summary>
    [DebuggerDisplay("If: {Condition}")]
    public sealed class If
    {
        /// <summary>Initializes a new instance of the <see cref="If"/> class.</summary>
        internal If(bool condition, EventCollection events)
            : this(condition ? IfState.True : IfState.False, events) { }

        /// <summary>Initializes a new instance of the <see cref="If"/> class.</summary>
        internal If(IfState state, EventCollection events)
        {
            State = state;
            Events = events;
        }

        /// <summary>The state of the if-branch.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal IfState State { get; }

        /// <summary>The parent <see cref="EventCollection"/>.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal EventCollection Events { get; }

        /// <summary>The event that should be added when the if-condition is true.</summary>
        /// <typeparam name="TEvent">
        /// The Type of the event to add.
        /// </typeparam>
        public Then Then<TEvent>(Func<TEvent> @event) where TEvent : class
        => State switch
        {
            IfState.True => new Then(true, Events.Add(@event())),
            IfState.False => new Then(false, Events),
            _ => new Then(true, Events),
        };
    }
}
