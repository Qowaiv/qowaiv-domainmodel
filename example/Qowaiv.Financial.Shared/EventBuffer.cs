using System;
using System.Collections.Generic;
using System.Linq;

namespace Qowaiv.Financial.Shared
{
    public class EventBuffer
    {
        private readonly List<object> buffer = new List<object>();
        private int offset;

        public EventBuffer(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }

        public object this[int version] => buffer[version - offset];

        public int CommittedVersion { get; private set; }

        public int Version => buffer.Count + offset;

        public IEnumerable<object> Committed => buffer.Take(CommittedVersion - offset);

        public IEnumerable<object> Uncommitted => buffer.Skip(CommittedVersion - offset);

        public bool HasUncommitted => Version != CommittedVersion;

        public EventBuffer AddRange(IEnumerable<object> events)
        {
            buffer.AddRange(events);
            return this;
        }

        /// <summary>Marks all events as being committed.</summary>
        public EventBuffer MarkAllAsCommitted()
        {
            CommittedVersion = Version;
            return this;
        }

        /// <summary>Removes the committed events from the stream.</summary>
        public EventBuffer ClearCommitted()
        {
            var delta = CommittedVersion - offset;
            buffer.RemoveRange(0, delta);
            offset += delta;
            return this;
        }
    }
}
