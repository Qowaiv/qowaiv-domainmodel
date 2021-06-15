using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Qowaiv.DomainModel.Events
{
    /// <summary>Represents a read-only collection of events.</summary>
    public partial class EventCollection
    {
        /// <summary><see cref="EventCollection"/> implementation for containing a group of events.</summary>
        private class Collection : NotEmpty
        {
            /// <summary>Initializes a new instance of the <see cref="Collection"/> class.</summary>
            public Collection(IEnumerable events, EventCollection predecessor)
                : base(predecessor) => Events = events;

            /// <summary>Events placeholder.</summary>
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private IEnumerable Events { get; }

            /// <inheritdoc />
            internal override IEnumerable<object> Enumerate()
                => base.Enumerate().Append(Events);
        }
    }
}
