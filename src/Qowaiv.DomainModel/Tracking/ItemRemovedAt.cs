using System.Collections.Generic;

namespace Qowaiv.DomainModel.Tracking
{
    /// <summary>Implements <see cref="ITrackableChange"/> for removing an element from a given index of an <see cref="IList{TChild}"/>.</summary>
    /// <typeparam name="TChild">
    /// The type of the elements of the collection.
    /// </typeparam>
    public class ItemRemovedAt<TChild> : ITrackableChange
    {
        /// <summary>Initializes a new instance of the <see cref="ItemRemovedAt{TChild}"/> class.</summary>
        /// <param name="collection">
        /// The collection to remove an item from.
        /// </param>
        /// <param name="index">
        /// The index of the position where the item should be removed.
        /// </param>
        /// <param name="item">
        /// The removed item.
        /// </param>
        public ItemRemovedAt(IList<TChild> collection, int index, TChild item)
        {
            this.collection = Guard.NotNull(collection, nameof(collection));
            this.index = index;
            this.item = item;
        }

        private readonly IList<TChild> collection;
        private readonly int index;
        private readonly TChild item;

        /// <inheritdoc />
        public void Apply() => collection.RemoveAt(index);

        /// <inheritdoc />
        public void Rollback() => collection.Insert(index, item);
    }
}
