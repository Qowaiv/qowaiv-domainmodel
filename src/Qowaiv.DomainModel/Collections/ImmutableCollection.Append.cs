namespace Qowaiv.DomainModel.Collections;

public partial class ImmutableCollection
{
    /// <remarks>Internal from .NET.</remarks>
    private const int MaxCapacity = 0X7FFFFFC7;

    [Pure]
    private ImmutableCollection Append(IEnumerator iterator)
    {
        var count = Count;
        var capacity = Capacity;
        var buffer = Buffer;
        var locker = Locker;
        var clone = true;

        if (count < capacity && buffer[count] is null)
        {
            lock (locker)
            {
                if (buffer[count] is null)
                {
                    while (count < capacity && iterator.MoveNext())
                    {
                        var item = iterator.Current;
                        if (item is null) continue;
                        buffer[count++] = item;
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

        while (iterator.MoveNext())
        {
            var item = iterator.Current;
            if (item is null) continue;
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
