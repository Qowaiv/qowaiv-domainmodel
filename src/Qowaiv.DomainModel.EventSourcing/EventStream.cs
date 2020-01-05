using Qowaiv.DomainModel.EventSourcing.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Qowaiv.DomainModel.EventSourcing
{
    /// <summary>An event stream.</summary>
    [DebuggerDisplay("{DebuggerDisplay}")]
    [DebuggerTypeProxy(typeof(CollectionDebugView))]
    public class EventStream : IEnumerable<EventMessage>
    {
        /// <summary>Underlying list.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<EventMessage> messages = new List<EventMessage>();

        /// <summary>Initializes a new instance of the <see cref="EventStream"/> class.</summary>
        public EventStream() : this(Guid.NewGuid()) { }

        /// <summary>Initializes a new instance of the <see cref="EventStream"/> class.</summary>
        /// <param name="aggregateId">
        /// The identifier of the aggregate root.
        /// </param>
        public EventStream(Guid aggregateId) => AggregateId = Guard.NotEmpty(aggregateId, nameof(aggregateId));

        /// <summary>Initializes a new instance of the <see cref="EventStream"/> class.</summary>
        /// <param name="aggregateId">
        /// The identifier of the aggregate root.
        /// </param>
        /// <param name="version">
        /// The initial version (offset).
        /// </param>
        internal EventStream(Guid aggregateId, int version) : this(aggregateId)
        {
            versionOffset = version;

            // Committed version can not be lower than the offset.
            CommittedVersion = version;
        }

        /// <summary>Gets the identifier of the aggregate linked to the event stream.</summary>
        public Guid AggregateId { get; }

        /// <summary>The version of the event stream.</summary>
        public int Version => messages.Count + versionOffset;

        private int versionOffset;

        /// <summary>Gets the committed version of the event stream.</summary>
        public int CommittedVersion { get; private set; }

        /// <summary>Returns true if all events together describe a (potential) full history for an aggregate.
        /// So all events in the stream are committed, and no event has been cleared (yet).
        /// </summary>
        public bool ContainsFullHistory
        {
            get => versionOffset == 0 && CommittedVersion == Version && Version > 0;
        }

        /// <summary>Gets a lock object to lock the event stream.</summary>
        public object Lock() => locker;

        private readonly object locker = new object();

        /// <summary>Adds an event to the event stream.</summary>
        /// <param name="event">
        /// The event to add.
        /// </param>
        public void Add(object @event)
        {
            Guard.NotNull(@event, nameof(@event));

            lock (locker)
            {
                var info = new EventInfo(Version + 1, AggregateId);
                var versioned = new EventMessage(info, @event);
                messages.Add(versioned);
            }
        }

        /// <summary>Adds an array of events to the event stream, all with the same version.</summary>
        /// <param name="events">
        /// The events to add.
        /// </param>
        public void AddRange(params object[] events)
        {
            Guard.HasAny(events, nameof(events));

            lock (locker)
            {
                var version = Version + 1;

                for (var i = 0; i < events.Length; i++)
                {
                    var @event = Guard.NotNull(events[i], $"events[{i}]");
                    var info = new EventInfo(version + i, AggregateId);
                    var versioned = new EventMessage(info, @event);
                    messages.Add(versioned);
                }
            }
        }

        /// <summary>Gets the uncommitted events of the event stream.</summary>
        public IEnumerable<EventMessage> GetUncommitted() => this.SkipWhile(message => message.Info.Version <= CommittedVersion);

        /// <summary>Marks all events as being committed (and clears the committed).</summary>
        public void MarkAllAsCommitted() => MarkAllAsCommitted(true);

        /// <summary>Marks all events as being committed.</summary>
        /// <param name="clearCommitted">
        /// if true, the committed events are cleared.
        /// </param>
        public void MarkAllAsCommitted(bool clearCommitted)
        {
            CommittedVersion = Version;
            if (clearCommitted)
            {
                ClearCommitted();
            }
        }

        /// <summary>Removes the committed events from the stream.</summary>
        public void ClearCommitted()
        {
            var delta = CommittedVersion - versionOffset;
            messages.RemoveRange(0, delta);
            versionOffset += delta;
        }

        /// <summary>Returns a <see cref="string"/> that represents the event stream for debug purposes.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal string DebuggerDisplay
        {
            get => Version == CommittedVersion
                ? $"Version: {Version}, Aggregate: {AggregateId:B}"
                : $"Version: {Version} (Committed: {CommittedVersion}), Aggregate: {AggregateId:B}";
        }

        /// <inheritdoc />
        public IEnumerator<EventMessage> GetEnumerator() => messages.GetEnumerator();

        /// <inheritdoc />
        [ExcludeFromCodeCoverage/* Just to satisfy the none-generic interface. */]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>Initializes the event stream with an initial set of events.</summary>
        /// <param name="events">
        /// The events that represent an event stream.
        /// </param>
        public static EventStream FromMessages(IEnumerable<EventMessage> events)
        {
            var eventArray = Guard.HasAny(events?.ToArray(), nameof(events));
            var first = eventArray[0].Info;

            for (var i = 0; i < eventArray.Length; i++)
            {
                var info = eventArray[i].Info;

                if (info.AggregateId != first.AggregateId)
                {
                    throw new ArgumentException(QowaivDomainModelMessages.ArgumentException_MultipleAggregates, nameof(events));
                }
                if (info.Version != i + 1)
                {
                    throw new EventsOutOfOrderException(nameof(events));
                }
            }
            var stream = new EventStream(first.AggregateId);
            stream.messages.AddRange(eventArray);
            stream.MarkAllAsCommitted(clearCommitted: false);
            return stream;
        }
    }
}
