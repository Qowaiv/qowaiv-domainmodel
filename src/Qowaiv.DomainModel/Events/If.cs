using System;
using System.Diagnostics;

namespace Qowaiv.DomainModel.Events
{
    /// <summary>Represents the start of an (logical) if-statement.</summary>
    [DebuggerDisplay("If: {Condition}")]
    public sealed class If
    {
        /// <summary>Initializes a new instance of the <see cref="If"/> class.</summary>
        internal If(bool condition, EventCollection predecessor)
        {
            Condition = condition;
            Predecessor = predecessor;
        }

        /// <summary>The to switch on.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool Condition { get; }

        /// <summary>The parent <see cref="EventCollection"/>.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private EventCollection Predecessor { get; }

        /// <summary>The event that should be added when the if-condition is true.</summary>
        /// <typeparam name="TEvent">
        /// The Type of the event to add.
        /// </typeparam>
        public Then<TEvent> Then<TEvent>(Func<TEvent> @event) where TEvent : class
            => new (this, @event);

        /// <summary>Adds an event/events to the collection.</summary>
        /// <param name="event">
        /// The event(s) to add.
        /// </param>
        /// <typeparam name="TEvent">
        /// The type of the event(s).
        /// </typeparam>
        internal EventCollection Add<TEvent>(TEvent @event) where TEvent : class
            => Predecessor.Add(@event);

        /// <summary>Implicitly casts an <see cref="If"/> to a <see cref="bool"/>.</summary>
        public static implicit operator bool(If @if) => @if?.Condition == true;
    }
}
