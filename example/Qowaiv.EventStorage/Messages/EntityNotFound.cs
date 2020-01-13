using Qowaiv.Validation.Abstractions;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Qowaiv.EventStorage.Messages
{
    /// <summary>Message to communicate if a entity could not be found.</summary>
    [Serializable]
    public class EntityNotFound : ISerializable, IValidationMessage
    {
        /// <summary>Initializes a new instance of the <see cref="EntityNotFound"/> class.</summary>
        public EntityNotFound() { }

        /// <summary>Initializes a new instance of the <see cref="EntityNotFound"/> class.</summary>
        protected EntityNotFound(SerializationInfo info, StreamingContext context)
        {
            Guard.NotNull(info, nameof(info));
            Message = info.GetString(nameof(Message));
        }

        /// <inheritdoc />
        public ValidationSeverity Severity => ValidationSeverity.Error;

        /// <inheritdoc />
        public string PropertyName => null;

        /// <inheritdoc />
        public string Message { get; set; }

        /// <inheritdoc />
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Guard.NotNull(info, nameof(info)).AddValue(nameof(Message), Message);
        }

        /// <summary>Creates an <see cref="EntityNotFound"/> for specific ID.</summary>
        public static EntityNotFound ForId(object id)
        {
            return new EntityNotFound { Message = string.Format(CultureInfo.InvariantCulture, "Entity with ID '{0}' could not be found.", id) };
        }
    }
}
