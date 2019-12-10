using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Qowaiv.DomainModel.EventSourcing
{
    /// <summary>Thrown when a provided <see cref="EventStream"/> does not describe a (potential) full history of events.</summary>
    [Serializable]
    public class EventStreamNoFullHistoryException : ArgumentException
    {
        /// <summary>Initializes a new instance of the <see cref="EventStreamNoFullHistoryException"/> class.</summary>
        /// <param name="paramName">
        /// The name of the parameter that causes this exception.
        /// </param>
        public EventStreamNoFullHistoryException(string paramName)
            : base(QowaivDomainModelMessages.EventStreamNoFullHistoryException, paramName) { }

        /// <summary>Initializes a new instance of the <see cref="EventStreamNoFullHistoryException"/> class.</summary>
        [ExcludeFromCodeCoverage/* Required Exception constructor for inheritance. */]
        public EventStreamNoFullHistoryException() { }

        /// <summary>Initializes a new instance of the <see cref="EventStreamNoFullHistoryException"/> class.</summary>
        /// <param name="message">
        /// The error message that explains the reason for this exception.
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference
        /// if no inner exception is specified.
        /// </param>
        [ExcludeFromCodeCoverage/* Required Exception constructor for inheritance. */]
        public EventStreamNoFullHistoryException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>Initializes a new instance of the <see cref="EventStreamNoFullHistoryException"/> class.</summary>
        /// <param name="message">
        /// The error message that explains the reason for this exception.
        /// </param>
        /// <param name="paramName">
        /// The name of the parameter that causes this exception.
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference
        /// if no inner exception is specified.
        /// </param>
        [ExcludeFromCodeCoverage/* Required Exception constructor for inheritance. */]
        public EventStreamNoFullHistoryException(string message, string paramName, Exception innerException) : base(message, paramName, innerException) { }

        /// <summary>Initializes a new instance of the <see cref="EventStreamNoFullHistoryException"/> class.</summary>
        /// <param name="info">
        /// The object that holds the serialized object data.
        /// </param>
        /// <param name="context">
        /// An object that describes the source or destination of the serialized data.
        /// </param>
        [ExcludeFromCodeCoverage/* Required Exception constructor for inheritance. */]
        protected EventStreamNoFullHistoryException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
