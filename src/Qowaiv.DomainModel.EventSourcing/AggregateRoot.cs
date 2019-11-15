﻿namespace Qowaiv.DomainModel.EventSourcing
{
    /// <summary>Factory to create <see cref="AggregateRoot{TAggregate}"/>s.</summary>
    public static class AggregateRoot
    {
        /// <summary>Loads an aggregate root from historical events.</summary>
        /// <remarks>
        /// When replaying historical events, validation is skipped.
        /// </remarks>
        public static TAggregate FromEvents<TAggregate>(EventStream stream)
             where TAggregate : EventSourcedAggregateRoot<TAggregate>, new()
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
