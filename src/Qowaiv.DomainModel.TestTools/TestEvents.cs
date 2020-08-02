using Qowaiv.DomainModel.TestTools.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Qowaiv.DomainModel.TestTools
{
    /// <summary>A collection of (test) events.</summary>
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(CollectionDebugView))]
    public class TestEvents : IEnumerable<object>
    {
        /// <summary>Gets an empty collection of test events.</summary>
        public static TestEvents Empty => new TestEvents();

        /// <summary>Creates a new collection of (test) events.</summary>
        /// <param name="events">
        /// Events to add.
        /// </param>
        /// <returns>
        /// A new collection of (test) events.
        /// </returns>
        public static TestEvents New(params object[] @events) => Empty.AddRange(events);

        private TestEvents() { }

        /// <summary>Gets the number of (test) events.</summary>
        public virtual int Count => 0;

        /// <summary>Adds a (test) event to a new collection.</summary>
        /// <param name="event">
        /// The event to add.
        /// </param>
        /// <returns>
        /// A new collection with the new event.
        /// </returns>
        public TestEvents Add(object @event)
            => new WithEvents(this, Guard.NotNull(@event, nameof(@event)));

        /// <summary>Adds (test) events to a new collection.</summary>
        /// <param name="events">
        /// The events to add.
        /// </param>
        /// <returns>
        /// A new collection with the new events.
        /// </returns>
        public TestEvents AddRange(IEnumerable<object> events)
        {
            Guard.NotNull(events, nameof(@events));

            var next = this;
            foreach (var @event in events)
            {
                next = next.Add(@event);
            }
            return next;
        }

        /// <summary>Adds the uncom
        /// 
        /// </summary>
        /// <typeparam name="TAggregate"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <param name="aggregate"></param>
        /// <returns></returns>
        public TestEvents AddUncommited<TAggregate, TId>(TAggregate aggregate)
            where TAggregate : AggregateRoot<TAggregate, TId>
            => AddRange(Guard.NotNull(aggregate, nameof(aggregate)).Buffer.Uncommitted);

        /// <summary>Creates the aggregate by replaying the (test) events, and the uncommited evenets of the aggregate.</summary>
        public TAggregate Load<TAggregate, TId>(TId id)
            where TAggregate : AggregateRoot<TAggregate, TId>, new()
            => AggregateRoot.FromStorage<TAggregate, TId>(new EventBuffer<TId>(id)
                .AddRange(this));

        /// <inheritdoc />
        public IEnumerator<object> GetEnumerator() => Enumerable().GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>Enumerates through all elements.</summary>
        internal virtual IEnumerable<object> Enumerable()
        {
            yield break;
        }

        private class WithEvents : TestEvents
        {
            internal WithEvents(TestEvents parent, object @event)
            {
                Parent = parent;
                Event = @event;
            }

            public override int Count => Parent.Count + 1;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private TestEvents Parent { get; }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private object Event { get; }

            internal override IEnumerable<object> Enumerable()
            {
                foreach (var @event in Parent)
                {
                    yield return @event;
                }
                yield return Event;
            }
        }
    }
}
