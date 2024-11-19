namespace Qowaiv.DomainModel;

/// <summary>Factory method for creating <see cref="AggregateRoot{TAggregate, TId}"/> from stored events.</summary>
[ExcludeFromCodeCoverage/* Justification = To help converting only. */]
[Obsolete("use Aggregate instead.")]
public static class AggregateRoot
{
    /// <summary>Creates an <see cref="AggregateRoot{TAggregate, TId}"/> from committed events.</summary>
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

        => Aggregate.FromStorage<TAggregate, TId>(buffer);
}

/// <summary>Represents an (domain-driven design) aggregate that is based on event sourcing.</summary>
/// <typeparam name="TAggregate">
/// The type of the aggregate root itself.
/// </typeparam>
[ExcludeFromCodeCoverage/* Justification = To help converting only. */]
[Obsolete("use Aggregate<TAggregate> instead.")]
public abstract class AggregateRoot<TAggregate> : Aggregate<TAggregate>
    where TAggregate : AggregateRoot<TAggregate>
{
    /// <summary>Initializes a new instance of the <see cref="AggregateRoot{TAggregate}"/> class.</summary>
    /// <param name="validator">
    /// A custom <paramref name="validator"/> to validate the aggregate.
    /// </param>
    protected AggregateRoot(IValidator<TAggregate> validator) : base(validator) { }
}

/// <summary>Represents an (domain-driven design) aggregate that is based on event sourcing.</summary>
/// <typeparam name="TAggregate">
/// The type of the aggregate root itself.
/// </typeparam>
/// <typeparam name="TId">
/// The type of the identifier.
/// </typeparam>
[ExcludeFromCodeCoverage/* Justification = To help converting only. */]
[Obsolete("use Aggregate<TAggregate, TId> instead.")]
public class AggregateRoot<TAggregate, TId> : Aggregate<TAggregate, TId>
    where TAggregate : AggregateRoot<TAggregate, TId>, new()
{
    /// <summary>Initializes a new instance of the <see cref="AggregateRoot{TAggregate, TId}"/> class.</summary>
    /// <param name="validator">
    /// A custom <paramref name="validator"/> to validate the aggregate.
    /// </param>
    /// <param name="aggregateId">
    /// The identifier of the aggregate.
    /// </param>
    protected AggregateRoot(TId aggregateId, IValidator<TAggregate> validator) : base(aggregateId, validator) { }
}
