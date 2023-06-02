namespace Qowaiv.DomainModel.Collections;

internal sealed class Singleton : IEnumerator, IEnumerable
{
    public Singleton(object current) => Current = current;

    public object Current { get; }

    private bool Done;

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
    [Pure]
    public IEnumerator GetEnumerator() => this;

    /// <inheritdoc />
    public void Reset() => throw new NotSupportedException();
}
