namespace Qowaiv.DomainModel.TestTools.Diagnostics;

/// <summary>Allows the debugger to display collections.</summary>
[ExcludeFromCodeCoverage]
internal sealed class CollectionDebugView
{
    /// <summary>Initializes a new instance of the <see cref="CollectionDebugView"/> class.</summary>
    /// <param name="enumeration">
    /// The collection to show the elements of.
    /// </param>
    public CollectionDebugView(IEnumerable enumeration) => this.enumeration = enumeration;

    /// <summary>The array that is shown by the debugger.</summary>
    /// <remarks>
    /// Every time the enumeration is shown in the debugger, a new array is created.
    /// By doing this, it is always in sync with the current state of the enumeration.
    /// </remarks>
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public object[] Items => enumeration.Cast<object>().ToArray();

    /// <summary>A reference to the enumeration to display.</summary>
    private readonly IEnumerable enumeration;
}
