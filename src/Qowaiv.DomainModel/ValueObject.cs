#pragma warning disable S4035 
// Classes implementing "IEquatable<T>" should be sealed
// It is left to actual implementations

using System;

namespace Qowaiv.DomainModel
{
    /// <summary>Represents an (domain-driven design) value object.</summary>
    /// <typeparam name="TValueObject">
    /// The type of the value object.
    /// </typeparam>
    /// <remarks>
    /// This base class should not be used for Single Value Objects (SVO's).
    /// </remarks>
    public abstract class ValueObject<TValueObject> : IEquatable<TValueObject> where TValueObject : ValueObject<TValueObject>
    {
        /// <inheritdoc />
        public abstract bool Equals(TValueObject other);

        /// <inheritdoc />
        public sealed override bool Equals(object obj) => obj is TValueObject other && Equals(other);

        /// <inheritdoc />
        public sealed override int GetHashCode() => Hash();

        /// <summary>Gets a hash code for the value object.</summary>
        /// <remarks>
        /// The reason to have this abstract Hash() method, and seal the GetHashCode()
        /// method, it enforce a custom implementation of a hash function.
        /// </remarks>
        protected abstract int Hash();

        /// <summary>Returns true if the two value objects are equal, other false.</summary>
        public static bool operator ==(ValueObject<TValueObject> left, ValueObject<TValueObject> right)
        {
            if (left is null || right is null)
            {
                return ReferenceEquals(left, right);
            }
            return left.Equals(right);
        }

        /// <summary>Returns true if the two value objects are not equal, other false.</summary>
        public static bool operator !=(ValueObject<TValueObject> left, ValueObject<TValueObject> right) => !(left == right);

        /// <summary>Returns true if other value object is the same instance as this one.</summary>
        protected bool AreSame(TValueObject other) => ReferenceEquals(this, other);

        /// <summary>Returns true if the other value object is not null.</summary>
        protected static bool NotNull([ValidatedNotNull]TValueObject other) => !(other is null);
    }
}
