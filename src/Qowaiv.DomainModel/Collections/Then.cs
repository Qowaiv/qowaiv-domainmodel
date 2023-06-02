namespace Qowaiv.DomainModel.Collections;

/// <summary>Represents the conditional addition of an item/items, after the if-statement.</summary>
[WillBeSealed]
public class Then : ImmutableCollection
{
    /// <summary>Initializes a new instance of the <see cref="Then"/> class.</summary>
    internal Then(bool done, ImmutableCollection predecessor)
        : base(predecessor.Count, predecessor.Buffer, predecessor.Locker)
        => Done = done;

    /// <summary>The predecessor <see cref="ImmutableCollection"/>.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private bool Done { get; }

    /// <summary>Adds else-if condition.</summary>
    [Pure]
    public If ElseIf(bool condition)
        => Done
        ? new If(IfState.Done, this)
        : new If(condition, this);

    /// <summary>Creates a new <see cref="ImmutableCollection"/> with the item(s) to add
    /// if the condition was not met.</summary>
    /// <param name="item">
    /// The item(s) to add.
    /// </param>
    /// <typeparam name="TElseItem">
    /// The type of the event to add.
    /// </typeparam>
    /// <remarks>
    /// Null, and null items are ignored.
    /// </remarks>
    [Pure]
    public ImmutableCollection Else<TElseItem>(Func<TElseItem> item) where TElseItem : class
        => Done || item is null
        ? this
        : Add(item());
}
