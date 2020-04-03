using Qowaiv.DomainModel.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

    /// <summary>A function to convert the event (payload) from the stored event.</summary>
    /// <typeparam name="TId">
    /// The type of the identifier of the aggregate.
    /// </typeparam>
    /// <typeparam name="TStoredEvent">
    /// The Type of the stored event.
    /// </typeparam>
    /// <param name="aggregateId">
    /// The identifier of the aggregate.
    /// </param>
    /// <param name="version">
    /// The version of the event.
    /// </param>
    /// <param name="event">
    /// The event (payload).
    /// </param>
    /// <returns>
    /// The converted event.
    /// </returns>
    public delegate TStoredEvent ConvertToStoredEvent<in TId, out TStoredEvent>(TId aggregateId, int version, object @event);

    /// <summary>A buffer of events that should be added to an event stream.</summary>
    /// <typeparam name="TId">
    /// The type of the identifier of the aggregate.
    /// </typeparam>
    [DebuggerDisplay("{DebuggerDisplay}")]
    [DebuggerTypeProxy(typeof(CollectionDebugView))]
    public class EventBuffer<TId> : IEnumerable<object>
    {
        private readonly List<object> buffer = new List<object>();
        private int offset;

        /// <summary>Initializes a new instance of the <see cref="EventBuffer{TId}"/> class.</summary>
        /// <param name="aggregateId">
        /// The identifier of the aggregate root.
        /// </param>
        public EventBuffer(TId aggregateId) : this(aggregateId, 0) { }

        /// <summary>Initializes a new instance of the <see cref="EventBuffer{TId}"/> class.</summary>
        /// <param name="aggregateId">
        /// The identifier of the aggregate root.
        /// </param>
        /// <param name="version">
        /// The initial version (offset).
        /// </param>
        public EventBuffer(TId aggregateId, int version)
        {
            AggregateId = aggregateId;
            offset = version;

            // Committed version can not be lower than the offset.
            CommittedVersion = version;
        }

        /// <summary>Gets the identifier of the aggregate root.</summary>
        public TId AggregateId { get; }

        /// <summary>The version of the event buffer.</summary>
        public int Version => buffer.Count + offset;

        /// <summary>Gets the committed version of the event buffer.</summary>
        public int CommittedVersion { get; private set; }

        /// <summary>Get all committed events in the event buffer.</summary>
        public IEnumerable<object> Committed => buffer.Take(CommittedVersion - offset);

        /// <summary>Get all uncommitted events in the event buffer.</summary>
        public IEnumerable<object> Uncommitted => buffer.Skip(CommittedVersion - offset);

        /// <summary>Returns true if the event buffer contains at least one uncommitted event.</summary>
        public bool HasUncommitted => Version != CommittedVersion;

        /// <summary>Returns true if the buffer contains no events.</summary>
        public bool IsEmpty => buffer.Count == 0;

        /// <summary>Adds an events to the event buffer.</summary>
        /// <param name="event">
        /// The event to add.
        /// </param>
        public EventBuffer<TId> Add(object @event)
        {
            Guard.NotNull(@event, nameof(@event));
            buffer.Add(@event);
            return this;
        }

        /// <summary>Adds  events to the event buffer.</summary>
        /// <param name="events">
        /// The events to add.
        /// </param>
        public EventBuffer<TId> AddRange(IEnumerable<object> events)
        {
            Guard.NotNull(events, nameof(events));

            var index = 0;

            foreach (var @event in events)
            {
                Guard.NotNull(@event, $"events[{index++}]");
                buffer.Add(@event);
            }
            return this;
        }

        /// <summary>Marks all events as being committed.</summary>
        public EventBuffer<TId> MarkAllAsCommitted()
        {
            CommittedVersion = Version;
            return this;
        }

        /// <summary>Removes the committed events from the buffer.</summary>
        public EventBuffer<TId> ClearCommitted()
        {
            var delta = CommittedVersion - offset;
            buffer.RemoveRange(0, delta);
            offset += delta;
            return this;
        }

        /// <summary>Selects the uncommitted events.</summary>
        /// <typeparam name="TStoredEvent">
        /// The type to convert it to.
        /// </typeparam>
        /// <param name="convert">
        /// The function to convert a <typeparamref name="TStoredEvent"/> from
        /// the aggregate identifier and the version of the event.
        /// </param>
        /// <returns>
        /// The uncommitted events as <typeparamref name="TStoredEvent"/>.
        /// </returns>
        public IEnumerable<TStoredEvent> SelectUncommitted<TStoredEvent>(ConvertToStoredEvent<TId, TStoredEvent> convert)
        {
            Guard.NotNull(convert, nameof(convert));

            var version = CommittedVersion;
            foreach (var @event in Uncommitted)
            {
                yield return convert(AggregateId, ++version, @event);
            }
        }

        /// <inheritdoc/>
        public IEnumerator<object> GetEnumerator() => buffer.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>Returns a <see cref="string"/> that represents the event buffer for debug purposes.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal string DebuggerDisplay
        {
            get => Version == CommittedVersion
                ? $"Version: {Version}, Aggregate: {AggregateId}"
                : $"Version: {Version} (Committed: {CommittedVersion}), Aggregate: {AggregateId}";
        }

        /// <summary>Creates an event buffer from some storage.</summary>
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
        public static EventBuffer<TId> FromStorage<TStoredEvent>(
            TId aggregateId,
            IEnumerable<TStoredEvent> storedEvents,
            ConvertFromStoredEvent<TStoredEvent> convert)
        {
            return FromStorage(aggregateId, 0, storedEvents, convert);
        }

        /// <summary>Creates an event buffer from some storage.</summary>
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
        public static EventBuffer<TId> FromStorage<TStoredEvent>(
            TId aggregateId,
            int initialVersion,
            IEnumerable<TStoredEvent> storedEvents,
            ConvertFromStoredEvent<TStoredEvent> convert)
        {
            Guard.NotNegative(initialVersion, nameof(initialVersion));
            Guard.NotNull(storedEvents, nameof(storedEvents));
            Guard.NotNull(convert, nameof(convert));

            var eventBuffer = new EventBuffer<TId>(aggregateId, initialVersion);

            foreach (var storedEvent in storedEvents)
            {
                eventBuffer.Add(convert(storedEvent));
            }
            eventBuffer.MarkAllAsCommitted();

            return eventBuffer;
        }
    }
}
