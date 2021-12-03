namespace Qowaiv.DomainModel.Collections;

/// <summary>Represents the conditional addition of an item/items, after the if-statement.</summary>
public class Then : ImmutableCollection
{
    /// <summary>Initializes a new instance of the <see cref="Then"/> class.</summary>
    internal Then(bool done, ImmutableCollection predecessor)
    {
        Done = done;
        Predecessor = predecessor;
    }

    /// <summary>The predecessor <see cref="ImmutableCollection"/>.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private ImmutableCollection Predecessor { get; }

    /// <summary>The predecessor <see cref="ImmutableCollection"/>.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private bool Done { get; }

    /// <summary>Adds else-if condition.</summary>
    [Pure]
    public If ElseIf(bool condition)
        => Done
        ? new If(IfState.Done, Predecessor)
        : new If(condition, Predecessor);

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
        ? Predecessor
        : Predecessor.Add(item());

    /// <inheritdoc/>
    [Pure]
    internal override IEnumerable<object> Enumerate()
        => Predecessor.Enumerate();
}
