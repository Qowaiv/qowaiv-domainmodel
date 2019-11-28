using System;
using System.Collections.Generic;
using System.Reflection;

namespace Qowaiv.DomainModel.TestTools
{
    /// <summary>Compares events.</summary>
    /// <remarks>
    /// Events are assumed to be equal if the have the same type, and all public properties are equal.
    /// </remarks>
    public class EventEqualityComparer : IEqualityComparer<object>
    {
        /// <summary>Gets a singleton instance of an <see cref="EventEqualityComparer"/>.</summary>
        public static readonly EventEqualityComparer Instance = new EventEqualityComparer();

        /// <inheritdoc />
        public new bool Equals(object x, object y)
        {
            if (x is null || y is null)
            {
                return x == y;
            }
            if (x.GetType() != y.GetType())
            {
                return false;
            }

            var properties = x.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                var _x = prop.GetValue(x, Array.Empty<object>());
                var _y = prop.GetValue(y, Array.Empty<object>());

                if (!object.Equals(_x, _y))
                {
                    return false;
                }
            }
            return true;
        }

        /// <inheritdoc />
        public int GetHashCode(object obj) => obj is null ? 0 : obj.GetType().GetHashCode();
    }
}
