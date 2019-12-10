using System.Collections.Generic;

namespace Qowaiv.DomainModel.Tracking
{
    /// <summary>Implements <see cref="ITrackableChange"/> for inserting an element at a given index of an <see cref="IList{TChild}"/>.</summary>
    /// <typeparam name="TChild">
    /// The type of the elements of the collection.
    /// </typeparam>
    public class ItemInserted<TChild> : ITrackableChange
    {
        /// <summary>Initializes a new instance of the <see cref="ItemInserted{TChild}"/> class.</summary>
        /// <param name="collection">
        /// The collection where an item will be inserted in.
        /// </param>
        /// <param name="index">
        /// The index of the position where the item should be inserted.
        /// </param>
        /// <param name="item">
        /// The item to insert.
        /// </param>
        public ItemInserted(IList<TChild> collection, int index, TChild item)
        {
            this.collection = Guard.NotNull(collection, nameof(collection));
            this.index = index;
            this.item = item;
        }

        private readonly IList<TChild> collection;
        private readonly int index;
        private readonly TChild item;

        /// <inheritdoc />
        public void Apply() => collection.Insert(index, item);

        /// <inheritdoc />
        public void Rollback() => collection.RemoveAt(index);
    }
}
