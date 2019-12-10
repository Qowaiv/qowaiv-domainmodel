using System.Collections.Generic;

namespace Qowaiv.DomainModel.Tracking
{
    /// <summary>Implements <see cref="ITrackableChange"/> sorting the elements of a <see cref="List{TChild}"/>.</summary>
    /// <typeparam name="TChild">
    /// The type of list that should be sorted.
    /// </typeparam>
    public class CollectionSorted<TChild> : ITrackableChange
    {
        /// <summary>Initializes a new instance of the <see cref="CollectionSorted{TChild}"/> class.</summary>
        /// <param name="collection">
        /// The collection that should be sorted.
        /// </param>
        /// <param name="comparer">
        /// The comparer to use for sorting the collection.
        /// </param>
#pragma warning disable S3956 // "Generic.List" instances should not be part of public APIs
        public CollectionSorted(List<TChild> collection, IComparer<TChild> comparer)
#pragma warning restore S3956 // "Generic.List" instances should not be part of public APIs
        {
            this.collection = Guard.NotNull(collection, nameof(collection));
            this.comparer = comparer ?? Comparer<TChild>.Default;
        }

        private readonly List<TChild> collection;
        private readonly IComparer<TChild> comparer;
        private TChild[] original;

        /// <inheritdoc />
        public void Apply()
        {
            original = collection.ToArray();
            collection.Sort(comparer);
        }

        /// <inheritdoc />
        public void Rollback()
        {
            collection.Clear();
            collection.AddRange(original);
        }
    }
}
