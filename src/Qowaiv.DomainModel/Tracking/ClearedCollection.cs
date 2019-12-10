using System.Collections.Generic;
using System.Linq;

namespace Qowaiv.DomainModel.Tracking
{
    /// <summary>Implements <see cref="ITrackableChange"/> for removing all elements from a <see cref="ICollection{TChild}"/>.</summary>
    /// <typeparam name="TChild">
    /// The type of the items of the collection.
    /// </typeparam>
    public class ClearedCollection<TChild> : ITrackableChange
    {
        /// <summary>Initializes a new instance of the <see cref="ClearedCollection{TChild}"/> class.</summary>
        /// <param name="collection">
        /// The collection that should be cleared.
        /// </param>
        public ClearedCollection(ICollection<TChild> collection)
        {
            this.collection = Guard.NotNull(collection, nameof(collection));
            items = this.collection.ToArray();
        }

        private readonly ICollection<TChild> collection;
        private readonly TChild[] items;

        /// <inheritdoc />
        public void Apply() => collection.Clear();

        /// <inheritdoc />
        public void Rollback()
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
    }
}
