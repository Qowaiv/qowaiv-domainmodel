namespace Qowaiv.DomainModel.Collections;

/// <summary>Converts a single element into a set with one item.</summary>
internal struct Singleton : IEnumerator
{
    /// <summary>Initializes a new instance of the <see cref="Singleton"/> struct.</summary>
    public Singleton(object element) => Current = element;

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
    public void Reset() => throw new NotSupportedException();
}
