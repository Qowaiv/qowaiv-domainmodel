namespace Qowaiv.DomainModel;

/// <summary>Factory method for creating <see cref="Aggregate{TAggregate, TId}"/> from stored events.</summary>
public static class Aggregate
{
    /// <summary>Creates an <see cref="Aggregate{TAggregate, TId}"/> from committed events.</summary>
    /// <typeparam name="TAggregate">
    /// The type of aggregate to create.
    /// </typeparam>
    /// <typeparam name="TId">
    /// The type of identifier of the aggregate.
    /// </typeparam>
    /// <param name="buffer">
    /// The buffer to replay.
    /// </param>
    [Pure]
    public static TAggregate FromStorage<TAggregate, TId>(EventBuffer<TId> buffer)
        where TAggregate : Aggregate<TAggregate, TId>, new()
    {
        Guard.HasAny(buffer, nameof(buffer));

        var aggregate = new TAggregate();
        aggregate.Replay(buffer);
        return aggregate;
    }
}
