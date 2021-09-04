using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Qowaiv.DomainModel.Collections
{
    /// <summary>Represents a read-only collection of events.</summary>
    public partial class ImmutableCollection
    {
        /// <summary>Not empty <see cref="ImmutableCollection"/> implementation.</summary>
        private class NotEmpty : ImmutableCollection
        {
            /// <summary>Initializes a new instance of the <see cref="NotEmpty"/> class.</summary>
            protected NotEmpty(ImmutableCollection predecessor) => Predecessor = predecessor;

            /// <summary>The predecessor <see cref="ImmutableCollection"/>.</summary>
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private ImmutableCollection Predecessor { get; }

            /// <inheritdoc />
            [Pure]
            internal override IEnumerable<object> Enumerate()
                => Predecessor.Enumerate();
        }
    }
}
