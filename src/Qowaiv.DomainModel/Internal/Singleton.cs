namespace Qowaiv.DomainModel.Internal;

/// <summary>Converts a single element into a set with one item.</summary>
internal struct Singleton : IReadOnlyCollection<object>, IEnumerator<object>
{
    /// <summary>Initializes a new instance of the <see cref="Singleton"/> struct.</summary>
    public Singleton(object element) => Current = element;

    /// <inheritdoc />
    public int Count => 1;

    /// <inheritdoc />
    public object Current { get; }

    /// <summary>Indicates if <see cref="MoveNext()"/> has been called.</summary>
    private bool Done;

    /// <inheritdoc />
    [Pure]
    public bool MoveNext()
    {
        if (!Done)
        {
            Done = true;
            return true;
        }
        else return false;
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage/* Required for backward comparability. */]
    public void Reset() => throw new NotSupportedException();

    /// <inheritdoc />
    [Pure]
    public IEnumerator<object> GetEnumerator() => this;

    /// <inheritdoc />
    [Pure]
    [ExcludeFromCodeCoverage/* Required for backward comparability. */]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public void Dispose() { /* Nothing to Dispose. */ }
}
