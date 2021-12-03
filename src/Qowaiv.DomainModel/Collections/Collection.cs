namespace Qowaiv.DomainModel.Collections;

/// <summary>Represents a read-only collection of events.</summary>
public partial class ImmutableCollection
{
    /// <summary><see cref="ImmutableCollection"/> implementation for containing a group of items.</summary>
    private sealed class Collection : NotEmpty
    {
        /// <summary>Initializes a new instance of the <see cref="Collection"/> class.</summary>
        public Collection(IEnumerable items, ImmutableCollection predecessor)
            : base(predecessor)
        {
            Items = new();
            foreach (var item in items)
            {
                Items.Add(item);
            }
        }

        /// <summary>Events placeholder.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<object> Items;

        /// <inheritdoc/>
        [Pure]
        internal override IEnumerable<object> Enumerate()
            => base.Enumerate().Append(Items);
    }
}
