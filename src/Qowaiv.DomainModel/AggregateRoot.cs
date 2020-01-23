namespace Qowaiv.DomainModel
{
    /// <summary>Factory to create <see cref="AggregateRoot{TAggregate}"/>s.</summary>
    public static class AggregateRoot
    {
        /// <summary>Loads an aggregate root from historical events.</summary>
        /// <param name="stream">
        /// The event stream containing the full history.
        /// </param>
        /// <typeparam name="TAggregate">
        /// The type of the aggregate.
        /// </typeparam>
        /// <remarks>
        /// When replaying historical events, validation is skipped.
        /// </remarks>
        public static TAggregate FromEvents<TAggregate>(EventStream stream)
             where TAggregate : AggregateRoot<TAggregate>, new()
        {
            Guard.NotNull(stream, nameof(stream));

            if (!stream.ContainsFullHistory)
            {
                throw new EventStreamNoFullHistoryException(nameof(stream));
            }
            var aggregateRoot = new TAggregate();
            aggregateRoot.LoadEvents(stream);
            return aggregateRoot;
        }
    }
}
