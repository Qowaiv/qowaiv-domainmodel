using System.Runtime.CompilerServices;

namespace Qowaiv.DomainModel.Collections;

[DebuggerTypeProxy(typeof(CollectionDebugView))]
[DebuggerDisplay("Count = {Count}, Capacity = {Buffer.Length}")]
internal readonly struct AppendOnlyCollection : IEnumerable<object>
{
    public static readonly AppendOnlyCollection Empty = new(0, Array.Empty<object>());

    /// <remarks>Internal from .NET.</remarks>
    private const int MaxCapacity = 0X7FFFFFC7;

    /// <summary>Initializes a new instance of the <see cref="AppendOnlyCollection"/> struct.</summary>
    private AppendOnlyCollection(int count, object[] buffer)
    {
        Count = count;
        Buffer = buffer;
    }

    public readonly int Count;
    internal readonly object[]? Buffer;

    /// <summary>Creates a new <see cref="AppendOnlyCollection"/> with the added item(s).</summary>
    /// <param name="item">
    /// The item(s) to add.
    /// </param>
    /// <remarks>
    /// Null, and null items are ignored.
    /// </remarks>
    [Pure]
    public AppendOnlyCollection Add(object? item)
    {
        var added = Buffer is null ? Empty : this;

        if (item is IEnumerable elements && item is not string)
        {
            foreach (var element in elements)
            {
                added = added.Append(element);
            }
            return added;
        }
        else
        {
            return added.Append(item);
        }
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private AppendOnlyCollection Append(object? element)
    {
        if (element is null)
        {
            return this;
        }
        else if (Buffer!.Length <= Count || Buffer[Count] is not null)
        {
            return Copy(Buffer, element);
        }

        lock (Buffer)
        {
            if (Buffer.Length > Count && Buffer[Count] is null)
            {
                Buffer[Count] = element;
                return new(Count + 1, Buffer);
            }
        }
        return Copy(Buffer, element);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private AppendOnlyCollection Copy(object[] buffer, object element)
    {
        int capacity = buffer.Length == 0 ? 8 : 2 * buffer.Length;

        // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
        // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
        if ((uint)capacity > MaxCapacity) capacity = MaxCapacity;

        var copy = new object[capacity];
        Array.Copy(buffer, copy, Count);
        copy[Count] = element;

        return new(Count + 1, copy);
    }

    /// <summary>Returns a specified range of contiguous elements from the collection.</summary>
    [Pure]
    public Enumerator Take(int count) => new(Buffer, Math.Min(count, Count));

    /// <summary>
    /// Bypasses a specified number of elements in the collection and then returns the remaining elements.
    /// </summary>
    [Pure]
    public Enumerator Skip(int count) => new(Buffer, count, Count);

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator()" />
    [Pure]
    public Enumerator GetEnumerator() => new(Buffer, Count);

    /// <inheritdoc />
    [Pure]
    IEnumerator<object> IEnumerable<object>.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
