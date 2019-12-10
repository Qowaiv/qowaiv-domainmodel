using System.Collections.Generic;

namespace Qowaiv.DomainModel.Tracking
{
    /// <summary>Implements <see cref="ITrackableChange"/> for updating an element at a specified index for an <see cref="IList{TChild}"/>.</summary>
    /// <typeparam name="TChild">
    /// The type of the element which has been updated.
    /// </typeparam>
    public class IndexUpdated<TChild> : ITrackableChange
    {
        /// <summary>Initializes a new instance of the <see cref="IndexUpdated{TChild}"/> class.</summary>
        /// <param name="collection">
        /// The collection with an updated item at a given index.
        /// </param>
        /// <param name="index">
        /// The index of the collection with an updated item.
        /// </param>
        /// <param name="orginal">
        /// The original value of the item at the specified index.
        /// </param>
        /// <param name="updated">
        /// The updated value of the item at the specified index.
        /// </param>
        public IndexUpdated(IList<TChild> collection, int index, TChild orginal, TChild updated)
        {
            this.collection = Guard.NotNull(collection, nameof(collection));
            this.index = index;
            this.orginal = orginal;
            this.updated = updated;
        }

        private readonly IList<TChild> collection;
        private readonly int index;
        private readonly TChild orginal;
        private readonly TChild updated;

        /// <inheritdoc />
        public void Apply() => collection[index] = updated;

        /// <inheritdoc />
        public void Rollback() => collection[index] = orginal;
    }
}
