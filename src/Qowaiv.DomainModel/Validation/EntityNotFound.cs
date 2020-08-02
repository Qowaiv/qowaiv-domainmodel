using Qowaiv.Validation.Abstractions;
using System;
using System.Runtime.Serialization;

namespace Qowaiv.DomainModel.Validation
{
    /// <summary>Message to communicate if a entity could not be found.</summary>
    [Serializable]
    public class EntityNotFound : InvalidOperationException, IValidationMessage
    {
        /// <summary>Initializes a new instance of the <see cref="EntityNotFound"/> class.</summary>
        public EntityNotFound() : this(QowaivDomainModelMessages.EntityNotFound) { }

        /// <summary>Initializes a new instance of the <see cref="EntityNotFound"/> class.</summary>
        public EntityNotFound(string message) : base(message) { }

        /// <summary>Initializes a new instance of the <see cref="EntityNotFound"/> class.</summary>
        public EntityNotFound(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>Initializes a new instance of the <see cref="EntityNotFound"/> class.</summary>
        protected EntityNotFound(SerializationInfo info, StreamingContext context) : base(info, context) { }

        /// <inheritdoc />
        public ValidationSeverity Severity => ValidationSeverity.Error;

        /// <inheritdoc />
        public string PropertyName => null;

        /// <summary>Creates an <see cref="EntityNotFound"/> for specific ID.</summary>
        public static EntityNotFound ForId(object id) =>
            new EntityNotFound(string.Format(QowaivDomainModelMessages.EntityNotFound_ForId, id));
    }
}
