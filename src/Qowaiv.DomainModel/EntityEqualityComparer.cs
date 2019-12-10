using System;
using System.Collections.Generic;

namespace Qowaiv.DomainModel
{
    /// <summary>Compares entities.</summary>
    /// <typeparam name="TEntity">
    /// The type of the entity to compare.
    /// </typeparam>
    public class EntityEqualityComparer<TEntity> : IEqualityComparer<TEntity>
        where TEntity : class, IEntity<TEntity>
    {
        /// <summary>Returns true if both entities have the same type, and have
        /// the same id, otherwise false.
        /// </summary>
        /// <param name="x">
        /// The first entity to compare.
        /// </param>
        /// <param name="y">
        /// The second entity to compare.
        /// </param>
        public bool Equals(TEntity x, TEntity y)
        {
            if (x is null || y is null)
            {
                return ReferenceEquals(x, y);
            }

            return
                x.GetType().Equals(y.GetType()) &&
                x.Id.Equals(y.Id);
        }

        /// <summary>Gets a hash code for the entity.</summary>
        /// <exception cref="ArgumentNullException">
        /// If the entity is null.
        /// </exception>
        public int GetHashCode(TEntity obj)
        {
            Guard.NotNull(obj, nameof(obj));
            return obj.Id.GetHashCode() ^ obj.GetType().GetHashCode();
        }
    }
}
