namespace Qowaiv.DomainModel.Collections;

/// <summary>Represents an immutable collection.</summary>
/// <remarks>
/// As a design choice, adding null is ignored. Also <see cref="IEnumerable"/>s
/// are added as collections, expect for <see cref="string"/>.
/// </remarks>
[Inheritable]
[DebuggerDisplay("Count: {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView))]
public partial class ImmutableCollection : IReadOnlyCollection<object>
{
    /// <summary>Gets an empty immutable collection.</summary>
    public static readonly ImmutableCollection Empty = new(0, Array.Empty<object>(), new());

    internal readonly object[] Buffer;
    internal readonly object Locker;

    /// <summary>Initializes a new instance of the <see cref="ImmutableCollection"/> class.</summary>
    internal ImmutableCollection(int count, object[] buffer, object locker)
    {
        Count = count;
        Buffer = buffer;
        Locker = locker;
    }

    /// <inheritdoc />
    public int Count { get; }

    /// <summary>Gets the capacity of the collection.</summary>
    internal int Capacity => Buffer.Length;

    /// <summary>Returns a specified range of contiguous elements from the collection.</summary>
    [Pure]
    public Enumerator Take(int count) => new(Buffer, Math.Min(count, Count));

    /// <summary>
    /// Bypasses a specified number of elements in the collection and then returns the remaining elements.
    /// </summary>
    [Pure]
    public Enumerator Skip(int count) => new(Buffer, count, Count);

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
    {
        if (item is null)
        {
            return this;
        }
        else
        {
            return item is string || item is not IEnumerable enumerable
                ? Append(new Singleton(item))
                : Append(enumerable.GetEnumerator());
        }
    }

    /// <summary>Starts a conditional addition.</summary>
    [Pure]
    public If If(bool? condition) => If(condition == true);

    /// <summary>Starts a conditional addition.</summary>
    [Pure]
    public If If(bool condition) => new(condition, this);

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator()" />
    [Pure]
    public Enumerator GetEnumerator() => new(Buffer, Count);

    /// <inheritdoc />
    [Pure]
    IEnumerator<object> IEnumerable<object>.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>Enumerator to iterate all elements in <see cref="ImmutableCollection"/>.</summary>
    public struct Enumerator : IEnumerator<object>, IEnumerable<object>
    {
        private readonly object[] Array;
        private readonly int End;
        private int Index;

        /// <summary>Initializes a new instance of the <see cref="Enumerator"/> struct.</summary>
        public Enumerator(object[] array, int start, int end)
        {
            Array = array;
            End = end;
            Index = start - 1;
        }

        /// <summary>Initializes a new instance of the <see cref="Enumerator"/> struct.</summary>
        public Enumerator(object[] array, int count) : this(array, 0, count) { }

        /// <inheritdoc />
        public object Current => Array[Index];

        /// <inheritdoc />
        [Pure]
        public bool MoveNext() => ++Index < End;

        /// <inheritdoc />
        public void Reset() => Index = -1;

        /// <inheritdoc />
        public void Dispose() { /* Nothing to dispose. */ }

        /// <inheritdoc />
        [Pure]
        public IEnumerator<object> GetEnumerator() => this;

        /// <inheritdoc />
        [Pure]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
