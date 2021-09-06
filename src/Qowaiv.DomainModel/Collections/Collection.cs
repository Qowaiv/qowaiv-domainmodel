using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Qowaiv.DomainModel.Collections
{
    /// <summary>Represents a read-only collection of events.</summary>
    public partial class ImmutableCollection
    {
        /// <summary><see cref="ImmutableCollection"/> implementation for containing a group of items.</summary>
        private class Collection : NotEmpty
        {
            /// <summary>Initializes a new instance of the <see cref="Collection"/> class.</summary>
            public Collection(IEnumerable item, ImmutableCollection predecessor)
                : base(predecessor) => Items = item;

            /// <summary>Events placeholder.</summary>
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private IEnumerable Items { get; }

            /// <inheritdoc/>
            [Pure]
            internal override IEnumerable<object> Enumerate()
                => base.Enumerate().Append(Items);
        }
    }
}
