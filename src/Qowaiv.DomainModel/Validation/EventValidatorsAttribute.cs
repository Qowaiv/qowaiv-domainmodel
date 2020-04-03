using System;
using System.Collections.Generic;

namespace Qowaiv.DomainModel.Validation
{
    /// <summary>Decorates a validator to carry event validators.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class EventValidatorsAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="EventValidatorsAttribute"/> class.</summary>
        /// <param name="eventValidator">
        /// The eventValidator to register.
        /// </param>
        public EventValidatorsAttribute(Type eventValidator)
        {
            Validators = new[] { Guard.NotNull(eventValidator, nameof(eventValidator)) };
        }

        /// <summary>Initializes a new instance of the <see cref="EventValidatorsAttribute"/> class.</summary>
        /// <param name="eventValidators">
        /// The eventValidators to register.
        /// </param>
        public EventValidatorsAttribute(params Type[] eventValidators)
        {
            Validators = Guard.HasAny(eventValidators, nameof(eventValidators));
        }

        /// <summary>Gets the validators.</summary>
        public IReadOnlyCollection<Type> Validators { get; }
    }
}
