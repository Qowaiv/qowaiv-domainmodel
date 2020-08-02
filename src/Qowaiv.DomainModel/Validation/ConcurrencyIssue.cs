using Qowaiv.Validation.Abstractions;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Qowaiv.DomainModel.Validation
{
    /// <summary>Message to communicate concurrency control failed.</summary>
    [Serializable]
    public class ConcurrencyIssue : InvalidOperationException, IValidationMessage
    {
        /// <summary>Initializes a new instance of the <see cref="ConcurrencyIssue"/> class.</summary>
        public ConcurrencyIssue()
            : base(QowaivDomainModelMessages.ConcurrencyIssue) { }

        /// <summary>Initializes a new instance of the <see cref="ConcurrencyIssue"/> class.</summary>
        protected ConcurrencyIssue(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        /// <summary>Initializes a new instance of the <see cref="ConcurrencyIssue"/> class.</summary>
        public ConcurrencyIssue(string message)
            : base(message) { }

        /// <summary>Initializes a new instance of the <see cref="ConcurrencyIssue"/> class.</summary>
        public ConcurrencyIssue(string message, Exception innerException)
            : base(message, innerException) { }

        /// <inheritdoc />
        public ValidationSeverity Severity => ValidationSeverity.Error;

        /// <inheritdoc />
        public string PropertyName => null;

        /// <summary>Creates an <see cref="ConcurrencyIssue"/> for the version mismatch.</summary>
        public static ConcurrencyIssue VersionMismatch(int expectedVersion, int actualVersion)
        {
            return new ConcurrencyIssue(string.Format(
                CultureInfo.CurrentCulture,
                QowaivDomainModelMessages.ConcurrencyIssue,
                expectedVersion,
                actualVersion));
        }

        /// <summary>Creates an <see cref="ConcurrencyIssue"/> for mid-air collision.</summary>
        public static ConcurrencyIssue MidAirCollision()
            => new ConcurrencyIssue(QowaivDomainModelMessages.MidAirCollision);
    }
}
