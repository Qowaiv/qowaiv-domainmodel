using System;
using System.Runtime.Serialization;

namespace EventStorage.Abstractions
{
    /// <summary>Exception thrown when no events could be found for the specified aggregate.</summary>
    [Serializable]
    public class AggregateNotFoundException : Exception
    {
        /// <summary>Creates a new instance of the <see cref="AggregateNotFoundException"/> object.</summary>
        public AggregateNotFoundException()
             : this("No Aggregate could be found.") { }

        /// <summary>Creates a new instance of the <see cref="AggregateNotFoundException"/> object.</summary>
        public AggregateNotFoundException(string message) : base(message) { }

        /// <summary>Creates a new instance of the <see cref="AggregateNotFoundException"/> object.</summary>
        public AggregateNotFoundException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>Creates a new instance of the <see cref="AggregateNotFoundException"/> object.</summary>
        protected AggregateNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            AggregateId = info?.GetString(nameof(AggregateId));
        }

        /// <inheritdoc/>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info?.AddValue(nameof(AggregateId), AggregateId);
        }

        /// <summary>The identifier that could not be found.</summary>
        public string AggregateId { get; private set; }

        /// <summary>Creates a new instance of the <see cref="AggregateNotFoundException"/> object for the specfied identifier.</summary>
        /// <typeparam name="TId">
        /// The type of th identifier.
        /// </typeparam>
        /// <param name="id">
        /// The identifier of aggregate that could not be found.
        /// </param>
        public static AggregateNotFoundException ForId<TId>(TId id) => new AggregateNotFoundException($"No Aggregate could be found with id '{id}'.");
    }
}
