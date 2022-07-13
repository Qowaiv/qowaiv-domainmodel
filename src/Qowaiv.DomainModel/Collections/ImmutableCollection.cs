namespace Qowaiv.DomainModel.Collections;

/// <summary>Represents an immutable collection.</summary>
/// <remarks>
/// As a design choice, adding null is ignored. Also <see cref="IEnumerable"/>s
/// are added as collections, expect for <see cref="string"/>.
/// </remarks>
[DebuggerDisplay("Count: {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView))]
public class ImmutableCollection : IReadOnlyCollection<object>
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
        IEnumerable enumerable => new Multiple(enumerable, this),
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

    /// <summary>Not empty <see cref="ImmutableCollection"/> implementation.</summary>
    private class NotEmpty : ImmutableCollection
    {
        /// <summary>Initializes a new instance of the <see cref="NotEmpty"/> class.</summary>
        protected NotEmpty(ImmutableCollection predecessor) => Predecessor = predecessor;

        /// <summary>The predecessor <see cref="ImmutableCollection"/>.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ImmutableCollection Predecessor { get; }

        /// <inheritdoc/>
        [Pure]
        internal override IEnumerable<object> Enumerate() => Predecessor.Enumerate();
    }

    /// <summary><see cref="ImmutableCollection"/> implementation for containing a single of item.</summary>
    private sealed class Single : NotEmpty
    {
        /// <summary>Initializes a new instance of the <see cref="Single"/> class.</summary>
        public Single(object item, ImmutableCollection predecessor) : base(predecessor) => Item = item;

        /// <summary>Item placeholder.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private object Item { get; }

        /// <inheritdoc/>
        [Pure]
        internal override IEnumerable<object> Enumerate() => base.Enumerate().Append(Item);
    }

    /// <summary><see cref="ImmutableCollection"/> implementation for containing a group of items.</summary>
    private sealed class Multiple : NotEmpty
    {
        /// <summary>Initializes a new instance of the <see cref="Multiple"/> class.</summary>
        public Multiple(IEnumerable items, ImmutableCollection predecessor) : base(predecessor)
            => Items = items.Cast<object>().Where(e => e is { }).ToArray();

        /// <summary>Events placeholder.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly object[] Items;

        /// <inheritdoc/>
        [Pure]
        internal override IEnumerable<object> Enumerate() => base.Enumerate().Concat(Items);
    }
}
