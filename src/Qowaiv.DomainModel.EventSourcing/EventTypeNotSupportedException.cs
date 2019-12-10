using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Qowaiv.DomainModel.EventSourcing
{
    /// <summary>The exception that is thrown when an event type is not supported for a specific <see cref="AggregateRoot{TAggrgate}"/>.</summary>
    [Serializable]
    public class EventTypeNotSupportedException : NotSupportedException
    {
        /// <summary>Initializes a new instance of the <see cref="EventTypeNotSupportedException"/> class.</summary>
        public EventTypeNotSupportedException(Type eventType, Type aggragateType)
            : this(GetMessage(eventType, aggragateType))
        {
            EventType = eventType;
            AggregateType = aggragateType;
        }

        private static string GetMessage(Type eventType, Type aggragateType)
        {
            return string.Format(
                QowaivDomainModelMessages.EventTypeNotSupportedException,
                eventType?.ToString() ?? "{null}",
                aggragateType ?? typeof(AggregateRoot<>));
        }

        /// <summary>Initializes a new instance of the <see cref="EventTypeNotSupportedException"/> class.</summary>
        public EventTypeNotSupportedException(string message) : base(message) { }

        /// <summary>Initializes a new instance of the <see cref="EventTypeNotSupportedException"/> class.</summary>
        protected EventTypeNotSupportedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Guard.NotNull(info, nameof(info));
            EventType = Type.GetType(info.GetString(nameof(EventType)));
            AggregateType = Type.GetType(info.GetString(nameof(AggregateType)));
        }

        /// <summary>Initializes a new instance of the <see cref="EventTypeNotSupportedException"/> class.</summary>
        [ExcludeFromCodeCoverage/* Required Exception constructor for inheritance. */]
        public EventTypeNotSupportedException() { }

        /// <summary>Initializes a new instance of the <see cref="EventTypeNotSupportedException"/> class.</summary>
        [ExcludeFromCodeCoverage/* Required Exception constructor for inheritance. */]
        public EventTypeNotSupportedException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>The event type that is not supported.</summary>
        public Type EventType { get; }

        /// <summary>The aggregate for which the event type is not supported.</summary>
        public Type AggregateType { get; }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(Guard.NotNull(info, nameof(info)), context);
            info.AddValue(nameof(EventType), EventType.AssemblyQualifiedName);
            info.AddValue(nameof(AggregateType), AggregateType.AssemblyQualifiedName);
        }
    }
}
