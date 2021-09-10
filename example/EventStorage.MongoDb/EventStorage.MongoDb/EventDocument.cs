using Qowaiv;
using System;
using System.Globalization;

namespace EventStorage.MongoDb
{
    internal class EventDocument
    {
        public string Id => AggregateId + ':' + Version.ToString(CultureInfo.InvariantCulture);
        public string AggregateId { get; set; }
        public int Version { get; set; }
        public DateTime CreatedUtc { get; set; } = Clock.UtcNow();
        public string EventType { get; set; }
        public object PayLoad { get; set; }
    }
}
