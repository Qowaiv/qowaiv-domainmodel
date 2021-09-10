using System;
using System.Runtime.Serialization;

namespace EventStorage.Abstractions
{
    /// <summary>Exception thrown when the expected version and the actual version in the storage are different.</summary>
    [Serializable]
    public class ConcurrencyException : Exception
    {
        /// <summary>Creates a new instance of the <see cref="ConcurrencyException"/> object.</summary>
        public ConcurrencyException(int expectedVersion, int actualVersion)
            : this($"A concurrency violation is encountered. Expected version {expectedVersion} but got {actualVersion}.") { }

        /// <summary>Creates a new instance of the <see cref="ConcurrencyException"/> object.</summary>
        public ConcurrencyException() : this("A concurrency violation is encountered.") { }

        /// <summary>Creates a new instance of the <see cref="ConcurrencyException"/> object.</summary>
        public ConcurrencyException(string message) : base(message) { }

        /// <summary>Creates a new instance of the <see cref="ConcurrencyException"/> object.</summary>
        public ConcurrencyException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>Creates a new instance of the <see cref="ConcurrencyException"/> object.</summary>
        protected ConcurrencyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
