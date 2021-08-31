﻿using Qowaiv.DomainModel.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Qowaiv.DomainModel
{
    /// <summary>A function to convert the stored event to the event (payload).</summary>
    /// <typeparam name="TStoredEvent">
    /// The Type of the stored event.
    /// </typeparam>
    /// <param name="storedEvent">
    /// The stored event.
    /// </param>
    /// <returns>
    /// The converted event (payload).
    /// </returns>
    public delegate object ConvertFromStoredEvent<in TStoredEvent>(TStoredEvent storedEvent);

    /// <summary><see cref="EventBuffer{TId}"/> factory helper.</summary>
    public static class EventBuffer
    {
        /// <summary>Gets an empty <see cref="EventBuffer{TId}"/>.</summary>
        /// <typeparam name="TId">
        /// The type of the identifier of the aggregate.
        /// </typeparam>
        /// <param name="aggregateId">
        /// The identifier of the aggregate root.
        /// </param>
        public static EventBuffer<TId> Empty<TId>(TId aggregateId) => Empty(aggregateId, 0);

        /// <summary>Gets an empty <see cref="EventBuffer{TId}"/>.</summary>
        /// <typeparam name="TId">
        /// The type of the identifier of the aggregate.
        /// </typeparam>
        /// <param name="aggregateId">
        /// The identifier of the aggregate root.
        /// </param>
        /// <param name="version">
        /// The initial version (offset).
        /// </param>
        public static EventBuffer<TId> Empty<TId>(TId aggregateId, int version)
            => new(aggregateId, offset: version, committed: version, ImmutableCollection.Empty);

        /// <summary>Creates an event buffer from some storage.</summary>
        /// <typeparam name="TId">
        /// The type of the identifier of the aggregate.
        /// </typeparam>
        /// <typeparam name="TStoredEvent">
        /// The type of the stored event.
        /// </typeparam>
        /// <param name="aggregateId">
        /// The identifier of the aggregate root.
        /// </param>
        /// <param name="storedEvents">
        /// The stored events.
        /// </param>
        /// <param name="convert">
        /// The function that select a 'clean' event from a stored event.
        /// </param>
        /// <returns>
        /// An event buffer with contains only committed events.
        /// </returns>
        public static EventBuffer<TId> FromStorage<TId, TStoredEvent>(
            TId aggregateId,
            IEnumerable<TStoredEvent> storedEvents,
            ConvertFromStoredEvent<TStoredEvent> convert)
        {
            return FromStorage(aggregateId, 0, storedEvents, convert);
        }

        /// <summary>Creates an event buffer from some storage.</summary>
        /// <typeparam name="TId">
        /// The type of the identifier of the aggregate.
        /// </typeparam>
        /// <typeparam name="TStoredEvent">
        /// The type of the stored event.
        /// </typeparam>
        /// <param name="aggregateId">
        /// The identifier of the aggregate root.
        /// </param>
        /// <param name="initialVersion">
        /// The initial version (offset).
        /// </param>
        /// <param name="storedEvents">
        /// The stored events.
        /// </param>
        /// <param name="convert">
        /// The function that select a 'clean' event from a stored event.
        /// </param>
        /// <returns>
        /// An event buffer with contains only committed events.
        /// </returns>
        public static EventBuffer<TId> FromStorage<TId, TStoredEvent>(
            TId aggregateId,
            int initialVersion,
            IEnumerable<TStoredEvent> storedEvents,
            ConvertFromStoredEvent<TStoredEvent> convert)
        {
            Guard.NotNegative(initialVersion, nameof(initialVersion));
            Guard.NotNull(storedEvents, nameof(storedEvents));
            Guard.NotNull(convert, nameof(convert));

            return Empty(aggregateId, initialVersion)
                .Add(storedEvents.Select(storedEvent => convert(storedEvent)))
                .MarkAllAsCommitted();
        }
    }
}
