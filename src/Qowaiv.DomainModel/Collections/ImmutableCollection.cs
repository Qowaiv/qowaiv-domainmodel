namespace Qowaiv.DomainModel.Collections;

/// <summary>Represents an immutable collection.</summary>
/// <remarks>
/// As a design choice, adding null is ignored. Also <see cref="IEnumerable"/>s
/// are added as collections, expect for <see cref="string"/>.
/// </remarks>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView))]
public readonly struct ImmutableCollection : IReadOnlyCollection<object>
{
    /// <summary>Gets an empty immutable collection.</summary>
    public static readonly ImmutableCollection Empty = new(AppendOnlyCollection.Empty);

    internal readonly AppendOnlyCollection Items;

    /// <summary>Initializes a new instance of the <see cref="ImmutableCollection"/> struct.</summary>
    internal ImmutableCollection(AppendOnlyCollection items) => Items = items;

    /// <inheritdoc />
    public int Count => Items.Count;

     /// <summary>Creates a new <see cref="ImmutableCollection"/> with the added items.</summary>
    /// <remarks>
    /// Null, and null items are ignored.
    /// </remarks>
    [Pure]
    public ImmutableCollection Add(params object?[] items) => Add<object[]>(items!);

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
    public ImmutableCollection Add<TItem>(TItem? item) where TItem : class
        => new(Items.Add(item));

    /// <summary>Starts a conditional addition.</summary>
    [Pure]
    public If If(bool? condition) => If(condition == true);

    /// <summary>Starts a conditional addition.</summary>
    [Pure]
    public If If(bool condition) => new(condition, this);

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator()" />
    [Pure]
    public Enumerator GetEnumerator() => Items.GetEnumerator();

    /// <inheritdoc />
    [Pure]
    IEnumerator<object> IEnumerable<object>.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
