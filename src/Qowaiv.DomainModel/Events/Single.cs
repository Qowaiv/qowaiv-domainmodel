using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Qowaiv.DomainModel.Events
{
    /// <summary>Represents a read-only collection of events.</summary>
    public partial class EventCollection
    {
        /// <summary><see cref="EventCollection"/> implementation for containing a single of event.</summary>
        private class Single : NotEmpty
        {
            /// <summary>Initializes a new instance of the <see cref="Single"/> class.</summary>
            public Single(object @event, EventCollection predecessor)
                : base(predecessor) => Event = @event;

            /// <summary>Event placeholder.</summary>
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private object Event { get; }

            /// <inheritdoc />
            protected override IEnumerable<object> Enumerate()
                => base.Enumerate().Append(Event);
        }
    }
}
