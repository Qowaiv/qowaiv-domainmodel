using Qowaiv.DomainModel.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Qowaiv.DomainModel.Collections
{
    /// <summary>Represents an immutable collection.</summary>
    /// <remarks>
    /// As a design choice, adding null is ignored. Also <see cref="IEnumerable"/>s
    /// are added as collections, expect for <see cref="string"/>.
    /// </remarks>
    [DebuggerDisplay("Count: {Count}")]
    [DebuggerTypeProxy(typeof(CollectionDebugView))]
    public partial class ImmutableCollection : IReadOnlyCollection<object>
    {
        /// <summary>Gets an empty event collection.</summary>
        public static readonly ImmutableCollection Empty = new();

        /// <summary>Initializes a new instance of the <see cref="ImmutableCollection"/> class.</summary>
        protected ImmutableCollection() { }

        /// <summary>Gets the total of events in the collection.</summary>
        public int Count => Enumerable.Count(this);

        /// <summary>Creates a new <see cref="ImmutableCollection"/> with the added items.</summary>
        /// <remarks>
        /// Null, and null items are ignored.
        /// </remarks>
        [Pure]
        public ImmutableCollection Add(params object[] items) => Add<object[]>(items);

        /// <summary>Creates a new <see cref="ImmutableCollection"/> with the added item(s).</summary>
        /// <param name="item">
        /// The item(s) to add.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of the item(s).
        /// </typeparam>
        /// <remarks>
        /// Null, and null items are ignored.
        /// </remarks>
        [Pure]
        public ImmutableCollection Add<TItem>(TItem item) where TItem : class
        => item switch
        {
            null => this,
            string => new Single(item, this),
            IEnumerable enumerable => new Collection(enumerable, this),
            _ => new Single(item, this),
        };

        /// <summary>Starts a conditional addition.</summary>
        [Pure]
        public If If(bool? condition) => If(condition == true);

        /// <summary>Starts a conditional addition.</summary>
        [Pure]
        public If If(bool condition) => new(condition, this);

        /// <inheritdoc/>
        [Pure]
        public IEnumerator<object> GetEnumerator()
            => Enumerate().GetEnumerator();

        /// <inheritdoc/>
        [Pure]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>Enumerates through all events.</summary>
        [Pure]
        internal virtual IEnumerable<object> Enumerate() => Enumerable.Empty<object>();
    }
}
