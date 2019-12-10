﻿using System;
using System.Globalization;

namespace Qowaiv.DomainModel.EventSourcing
{
    /// <summary>Contains information about an event.</summary>
    public struct EventInfo : IEquatable<EventInfo>
    {
        /// <summary>Represents empty/not set event info.</summary>
        public static readonly EventInfo Empy;

        /// <summary>Initializes a new instance of the <see cref="EventInfo"/> struct.</summary>
        /// <param name="version">
        /// The version of the event.
        /// </param>
        /// <param name="aggregateId">
        /// The identifier of the aggregate.
        /// </param>
        /// <param name="createdUtc">
        /// The date time (UTC) that the event has been created.
        /// </param>
        public EventInfo(int version, Guid aggregateId, DateTime createdUtc)
        {
            Version = version;
            AggregateId = Guard.NotEmpty(aggregateId, nameof(aggregateId));
            CreatedUtc = Guard.NotDefault(createdUtc, nameof(createdUtc));
        }

        /// <summary>Gets the version of event info.</summary>
        public int Version { get; }

        /// <summary>Gets the identifier of linked aggregate root.</summary>
        public Guid AggregateId { get; }

        /// <summary>Gets the creation date time (in UTC).</summary>
        public DateTime CreatedUtc { get; }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is EventInfo info && Equals(info);

        /// <inheritdoc />
        public bool Equals(EventInfo other)
        {
            return Version == other.Version &&
                AggregateId == other.AggregateId &&
                CreatedUtc == other.CreatedUtc;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Version.GetHashCode()
                ^ AggregateId.GetHashCode()
                ^ CreatedUtc.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, @"Version: {0}, {1:yyyy-MM-dd HH\:mm\:ss.FFFFF}, AggregateId: {2:B}", Version, CreatedUtc, AggregateId);
        }
    }
}
