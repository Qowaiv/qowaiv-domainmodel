using System.Runtime.CompilerServices;

namespace Qowaiv.DomainModel.Collections;

[DebuggerDisplay("Count = {Count}, Capacity = {Buffer?.Length}")]
internal readonly struct AppendOnlyCollection
{
    public static readonly AppendOnlyCollection Empty = new(0, Array.Empty<object>(), new());

    /// <remarks>Internal from .NET.</remarks>
    private const int MaxCapacity = 0X7FFFFFC7;

    /// <summary>Initializes a new instance of the <see cref="AppendOnlyCollection"/> struct.</summary>
    private AppendOnlyCollection(int count, object[] buffer, object locker)
    {
        Count = count;
        Buffer = buffer;
        Locker = locker;
    }

    public readonly int Count;
    internal readonly object[] Buffer;
    internal readonly object Locker;

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

    [Pure]
    private AppendOnlyCollection Append(IEnumerator iterator)
    {
        var count = Count;
        var buffer = Buffer ?? Array.Empty<object>();
        var capacity = buffer.Length;
        var locker = Locker ?? new();
        var clone = true;

        if (count < capacity && buffer[count] is null)
        {
            lock (locker)
            {
                if (buffer[count] is null)
                {
                    while (count < capacity && iterator.MoveNext())
                    {
                        if (iterator.Current is { } item)
                        {
                            buffer[count++] = item;
                        }
                    }
                    clone = count == capacity;
                }
            }
        }

        if (clone)
        {
            var copy = new object[capacity];
            Array.Copy(buffer, copy, count);
            buffer = copy;
            locker = new();
        }

        return Append(iterator, count, buffer, capacity, locker);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static AppendOnlyCollection Append(
        IEnumerator iterator,
        int count,
        object[] buffer,
        int capacity,
        object locker)
    {
        while (iterator.MoveNext())
        {
            if (iterator.Current is not { } item)
            {
                continue;
            }
            else if (count >= capacity)
            {
                buffer = Grow(buffer);
                capacity = buffer.Length;
                locker = new();
            }
            buffer[count++] = item;
        }
        return new(count, buffer, locker);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static object[] Grow(object[] array)
    {
        int capacity = array.Length == 0 ? 128 : 2 * array.Length;

        // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
        // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
        if ((uint)capacity > MaxCapacity) capacity = MaxCapacity;

        var grown = new object[capacity];
        Array.Copy(array, grown, array.Length);
        return grown;
    }
}
