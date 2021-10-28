using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace System.Linq
{
    /// <summary>Extensions on <see cref="IEnumerable{T}"/>.</summary>
    internal static class EnumerableExtensions
    {
        /// <summary>Append items.</summary>
        [Pure]
        public static IEnumerable<object> Append(this IEnumerable<object> collection, IEnumerable items)
            => collection.Concat(items.Cast<object>().Where(e => e is { }));

        /// <summary>Append a single item.</summary>
        [Pure]
        public static IEnumerable<object> Append(this IEnumerable<object> collection, object item)
            => item is null
            ? collection
            : collection.Concat(Enumerable.Repeat(item, 1));
    }
}
