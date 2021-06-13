using System.Collections.Generic;
using System.Diagnostics;

namespace Qowaiv.DomainModel.Events
{
    /// <summary>Represents a read-only collection of events.</summary>
    public partial class EventCollection
    {
        /// <summary>Not empty <see cref="EventCollection"/> implementation.</summary>
        private class NotEmpty : EventCollection
        {
            /// <summary>Initializes a new instance of the <see cref="NotEmpty"/> class.</summary>
            protected NotEmpty(EventCollection predecessor) => Predecessor = predecessor;

            /// <summary>The predecessor <see cref="EventCollection"/>.</summary>
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private EventCollection Predecessor { get; }

            /// <inheritdoc />
            protected override IEnumerable<object> Enumerate()
                => Predecessor.Enumerate();
        }
    }
}
