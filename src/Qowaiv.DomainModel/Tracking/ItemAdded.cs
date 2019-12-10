using System.Collections.Generic;

namespace Qowaiv.DomainModel.Tracking
{
    /// <summary>Implements <see cref="ITrackableChange"/> for adding an element to an <see cref="ICollection{TChild}"/>.</summary>
    /// <typeparam name="TChild">
    /// The of the item that has been added to a collection.
    /// </typeparam>
    public class ItemAdded<TChild> : ITrackableChange
    {
        /// <summary>Initializes a new instance of the <see cref="ItemAdded{TChild}"/> class.</summary>
        /// <param name="collection">
        /// The collection to add an item to.
        /// </param>
        /// <param name="item">
        /// The item to add.
        /// </param>
        public ItemAdded(IList<TChild> collection, TChild item)
        {
            this.collection = Guard.NotNull(collection, nameof(collection));
            this.item = item;
        }

        private readonly IList<TChild> collection;
        private readonly TChild item;

        /// <inheritdoc />
        public void Apply() => collection.Add(item);

        /// <inheritdoc />
        public void Rollback() => collection.RemoveAt(collection.Count - 1);
    }
}
