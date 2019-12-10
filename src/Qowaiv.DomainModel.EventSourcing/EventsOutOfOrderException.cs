using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Qowaiv.DomainModel.EventSourcing
{
    /// <summary>Thrown when events are out of order.</summary>
    [Serializable]
    public class EventsOutOfOrderException : ArgumentOutOfRangeException
    {
        /// <summary>Initializes a new instance of the <see cref="EventsOutOfOrderException"/> class.</summary>
        /// <param name="paramName">
        /// The name of the parameter that causes this exception.
        /// </param>
        public EventsOutOfOrderException(string paramName)
            : base(paramName, QowaivDomainModelMessages.EventsOutOfOrderException) { }

        /// <summary>Initializes a new instance of the <see cref="EventsOutOfOrderException"/> class.</summary>
        [ExcludeFromCodeCoverage/* Required Exception constructor for inheritance. */]
        public EventsOutOfOrderException() { }

        /// <summary>Initializes a new instance of the <see cref="EventsOutOfOrderException"/> class.</summary>
        /// <param name="message">
        /// The error message that explains the reason for this exception.
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference
        /// if no inner exception is specified.
        /// </param>
        [ExcludeFromCodeCoverage/* Required Exception constructor for inheritance. */]
        public EventsOutOfOrderException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>Initializes a new instance of the <see cref="EventsOutOfOrderException"/> class.</summary>
        /// <param name="info">
        /// The object that holds the serialized object data.
        /// </param>
        /// <param name="context">
        /// An object that describes the source or destination of the serialized data.
        /// </param>
        [ExcludeFromCodeCoverage/* Required Exception constructor for inheritance. */]
        protected EventsOutOfOrderException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
