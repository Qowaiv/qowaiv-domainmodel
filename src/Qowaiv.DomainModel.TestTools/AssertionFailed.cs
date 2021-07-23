using System;
using System.Runtime.Serialization;

namespace Qowaiv.DomainModel.TestTools
{
    /// <summary>Assert exception.</summary>
    /// <remarks>
    /// Exists to be independent to external test frameworks.
    /// </remarks>
    [Serializable]
    public class AssertionFailed : Exception
    {
        /// <summary>Initializes a new instance of the <see cref="AssertionFailed"/> class.</summary>
        public AssertionFailed() : this("Assertion failed.") { }

        /// <summary>Initializes a new instance of the <see cref="AssertionFailed"/> class.</summary>
        /// <param name="message">
        /// The error message that explains the reason for this exception.
        /// </param>
        public AssertionFailed(string message) : base(message) { }

        /// <summary>Initializes a new instance of the <see cref="AssertionFailed"/> class.</summary>
        /// <param name="message">
        /// The error message that explains the reason for this exception.
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference
        ///  if no inner exception is specified.
        /// </param>
        public AssertionFailed(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>Initializes a new instance of the <see cref="AssertionFailed"/> class.</summary>
        /// <param name="info">
        /// The object that holds the serialized object data.
        /// </param>
        /// <param name="context">
        /// An object that describes the source or destination of the serialized data.
        /// </param>
        protected AssertionFailed(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
