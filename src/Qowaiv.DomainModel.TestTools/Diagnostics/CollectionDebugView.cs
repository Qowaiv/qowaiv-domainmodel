namespace Qowaiv.DomainModel.TestTools.Diagnostics;

/// <summary>Allows the debugger to display collections.</summary>
[ExcludeFromCodeCoverage]
internal sealed class CollectionDebugView(IEnumerable enumeration)
{
    /// <summary>The array that is shown by the debugger.</summary>
    /// <remarks>
    /// Every time the enumeration is shown in the debugger, a new array is created.
    /// By doing this, it is always in sync with the current state of the enumeration.
    /// </remarks>
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public object[] Items => enumeration.Cast<object>().ToArray();

    /// <summary>A reference to the enumeration to display.</summary>
    private readonly IEnumerable enumeration = enumeration;
}
