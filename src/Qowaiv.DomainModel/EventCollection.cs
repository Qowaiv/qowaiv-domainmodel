using Qowaiv.DomainModel.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Qowaiv.DomainModel
{
    /// <summary>Represents a read-only collection of events.</summary>
    [DebuggerDisplay("Count: {Count}")]
    [DebuggerTypeProxy(typeof(CollectionDebugView))]
    public class EventCollection : IReadOnlyCollection<object>
    {
        /// <summary>Gets an empty collection.</summary>
        public static readonly EventCollection Empty = new EventCollection();

        /// <summary>Initializes a new instance of the <see cref="EventCollection"/> class.</summary>
        internal EventCollection() { }

        /// <summary>Initializes a new instance of the <see cref="EventCollection"/> class.</summary>
        protected EventCollection(EventCollection parent)
            => Parent = Guard.NotNull(parent, nameof(parent));

        /// <summary>Gets the total of events in the collection.</summary>
        public virtual int Count => Enumerable.Count(this);

        /// <summary>Gets the parent.</summary>
        /// <remarks>
        /// This allows to create a linked list with immutable nodes.
        /// </remarks>
        protected EventCollection Parent { get; }

        /// <summary>Adds a single event.</summary>
        /// <typeparam name="T">Type of the event.</typeparam>
        public EventCollection Add<T>(T @event) where T : class
            => @event is null
            ? this
            : new Single(@event, this);

        /// <summary>Adds a single event, but lazy.</summary>
        /// <typeparam name="T">Type of the event.</typeparam>
        public EventCollection Add<T>(Func<T> @event) where T : class
            => @event is null
            ? this
            : new Lazy<T>(@event, this);

        /// <summary>Adds a group of events.</summary>
        public EventCollection Add(params object[] events)
            => events is null || !events.Any(e => e is { })
            ? this
            : new Collection(events, this);

        /// <summary>Adds a group of events.</summary>
        public EventCollection Add(EventCollection events)
            => events is null || !events.Any()
            ? this
            : new Collection(events, this);

        /// <summary>Adds a group of events, but lazy.</summary>
        public EventCollection Add(Func<IEnumerable<object>> events)
            => events is null
            ? this
            : new LazyCollection(events, this);

        /// <summary>Undo's the last addition if not true.</summary>
        public EventCollection When(bool? condition)
            => When(condition == true);

        /// <summary>Undo's the last addition if false.</summary>
        public EventCollection When(bool condition)
            => condition ? this : Parent ?? Empty;

        /// <inheritdoc />
        public IEnumerator<object> GetEnumerator()
            => Enumerate().GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>Enumerates through all events.</summary>
        protected virtual IEnumerable<object> Enumerate()
            => Enumerable.Empty<object>();

        private class Single : EventCollection
        {
            public Single(object @event, EventCollection parent)
                : base(parent) => Event = @event;

            private object Event { get; }

            protected override IEnumerable<object> Enumerate()
                => base.Enumerate().Concat(Enumerable.Repeat(Event, 1));
        }

        private class Collection : EventCollection
        {
            public Collection(IEnumerable<object> events, EventCollection parent)
                : base(parent) => Events = events;

            private IEnumerable<object> Events { get; }

            protected override IEnumerable<object> Enumerate()
                => base.Enumerate().Concat(Events);
        }

        private class Lazy<T> : EventCollection
        {
            public Lazy(Func<T> @event, EventCollection parent)
                : base(parent) => Event = @event;

            private Func<T> Event { get; }

            protected override IEnumerable<object> Enumerate()
                => base.Enumerate().Concat(Enumerable.Repeat((object)Event(), 1));
        }

        private class LazyCollection : EventCollection
        {
            public LazyCollection(Func<IEnumerable<object>> events, EventCollection parent)
                : base(parent) => Events = events;

            private Func<IEnumerable<object>> Events { get; }

            protected override IEnumerable<object> Enumerate()
                => base.Enumerate().Concat(Events());
        }
    }
}
