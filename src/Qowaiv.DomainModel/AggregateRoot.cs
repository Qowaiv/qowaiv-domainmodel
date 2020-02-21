﻿using System.Collections.Generic;

namespace Qowaiv.DomainModel
{
    /// <summary>Factory method for creating <see cref="AggregateRoot{TAggregate, TId}"/> from stored events.</summary>
    public static class AggregateRoot
    {
        /// <summary>Creates an <see cref="AggregateRoot{TAggregate, TId}"/> from an event buffer.</summary>
        /// <typeparam name="TAggregate">
        /// The type of aggregate to create.
        /// </typeparam>
        /// <typeparam name="TId">
        /// The type of identifier of the aggregate.
        /// </typeparam>
        /// <param name="buffer">
        /// The buffer to replay.
        /// </param>
        public static TAggregate FromBuffer<TAggregate, TId>(EventBuffer<TId> buffer)
            where TAggregate : AggregateRoot<TAggregate, TId>, new()
        {
            var aggregate = new TAggregate();
            aggregate.Replay(buffer);
            return aggregate;
        }

        /// <summary>Creates an <see cref="AggregateRoot{TAggregate, TId}"/> from stored events.</summary>
        /// <typeparam name="TAggregate">
        /// The type of aggregate to create.
        /// </typeparam>
        /// <typeparam name="TId">
        /// The type of identifier of the aggregate.
        /// </typeparam>
        /// <typeparam name="TStoredEvent">
        /// The type of the stored event.
        /// </typeparam>
        /// <param name="aggregateId">
        /// The aggregate identifier.
        /// </param>
        /// <param name="storedEvents">
        /// The stored events.
        /// </param>
        /// <param name="convert">
        /// The function to select an event from an stored event.
        /// </param>
        public static TAggregate FromStorage<TAggregate, TId, TStoredEvent>(
            TId aggregateId,
            IEnumerable<TStoredEvent> storedEvents,
            ConvertFromStoredEvent<TStoredEvent> convert)
            where TAggregate : AggregateRoot<TAggregate, TId>, new()
        {
            return FromStorage<TAggregate, TId, TStoredEvent>(aggregateId, 0, storedEvents, convert);
        }

        /// <summary>Creates an <see cref="AggregateRoot{TAggregate, TId}"/> from stored events.</summary>
        /// <typeparam name="TAggregate">
        /// The type of aggregate to create.
        /// </typeparam>
        /// <typeparam name="TId">
        /// The type of identifier of the aggregate.
        /// </typeparam>
        /// <typeparam name="TStoredEvent">
        /// The type of the stored event.
        /// </typeparam>
        /// <param name="aggregateId">
        /// The aggregate identifier.
        /// </param>
        /// <param name="initialVersion">
        /// The initial version.
        /// </param>
        /// <param name="storedEvents">
        /// The stored events.
        /// </param>
        /// <param name="convert">
        /// The function to select an event from an stored event.
        /// </param>
        public static TAggregate FromStorage<TAggregate, TId, TStoredEvent>(
            TId aggregateId,
            int initialVersion,
            IEnumerable<TStoredEvent> storedEvents,
            ConvertFromStoredEvent<TStoredEvent> convert)
            where TAggregate : AggregateRoot<TAggregate, TId>, new()
        {
            var aggregate = new TAggregate();
            var buffer = EventBuffer<TId>.FromStorage(aggregateId, initialVersion, storedEvents, convert);
            aggregate.Replay(buffer);
            return aggregate;
        }
    }
}
