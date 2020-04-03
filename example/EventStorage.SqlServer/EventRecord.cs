using Dapper.Contrib.Extensions;
using System;

namespace EventStorage.SqlServer
{
    /// <summary>Represents an event record in the SQL storage.</summary>
    [Table("Events")]
    internal class EventRecord
    {
        public string AggregateId { get; set; }
        public int Version { get; set; }
        public DateTime CreatedUtc { get; set; }
        public string EventType { get; set; }
        public string Data { get; set; }
    }
}
