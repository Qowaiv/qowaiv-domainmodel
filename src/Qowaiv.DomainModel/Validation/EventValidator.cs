using Qowaiv.Validation.Abstractions;
using System;
using System.Linq;
using System.Reflection;

namespace Qowaiv.DomainModel.Validation
{
    /// <summary>Validator that can validate events before the will (potentially) be dispatched.</summary>
    internal static class EventValidator
    {
        private const string Validate = nameof(Validate);

        /// <summary>Validates the event and the aggregate combined in a <see cref="EventContext{TAggregate, TEvent}"/>.</summary>
        /// <typeparam name="TAggregate">
        /// The type of the aggregate.
        /// </typeparam>
        /// <param name="validator">
        /// The validator of the aggregate.
        /// </param>
        /// <param name="aggregate">
        /// The aggregate to validate.
        /// </param>
        /// <param name="event">
        /// The event to validate.
        /// </param>
        public static Result<TAggregate> ValidateEvent<TAggregate>(this IValidator<TAggregate> validator, TAggregate aggregate, object @event)
            where TAggregate : ValidatableAggregate<TAggregate>
        {
            var attribute = validator.GetType().GetCustomAttribute<EventValidatorsAttribute>();

            if (attribute is null)
            {
                return aggregate;
            }

            var interfaceType = GetValidatorType(typeof(TAggregate), @event.GetType());
            var validatorType = attribute.Validators.FirstOrDefault(type => type.Implements(interfaceType));

            if (validatorType is null)
            {
                return aggregate;
            }

            var eventValidator = Activator.CreateInstance(validatorType);
            var context = CreateContext(aggregate, @event);
            var validate = interfaceType.GetMethod(Validate);
            var result = (Result)validate.Invoke(eventValidator, new[] { context });

            return Result.For(aggregate, result.Messages.Select(StripPropertyName));
        }

        /// <summary>Creates a new instance of <see cref="EventContext{TAggregate, TEvent}"/>
        /// based on the <paramref name="aggregate"/> and the type of the <paramref name="event"/>.
        /// </summary>
        private static object CreateContext<TAggregate>(TAggregate aggregate, object @event)
        {
            var contextType = typeof(EventContext<,>).MakeGenericType(typeof(TAggregate), @event.GetType());
            return Activator.CreateInstance(contextType, aggregate, @event);
        }

        /// <summary>Gets The validator type <see cref="IValidator{TModel}"/> where TModel
        /// is a <see cref="EventContext{TAggregate, TEvent}"/>.
        /// </summary>
        private static Type GetValidatorType(Type aggregateType, Type eventType)
        {
            var contextType = typeof(EventContext<,>).MakeGenericType(aggregateType, eventType);
            return typeof(IValidator<>).MakeGenericType(contextType);
        }

        /// <summary>Strips the property name from the "Aggregate." prefix if present.</summary>
        private static IValidationMessage StripPropertyName(IValidationMessage message)
        {
            var prop = message.PropertyName;

            if (prop is null || !prop.StartsWith("Aggregate.", StringComparison.InvariantCultureIgnoreCase))
            {
                return message;
            }

            prop = prop.Substring(10);
            var mes = message.Message;

            return message.Severity switch
            {
                ValidationSeverity.Info => ValidationMessage.Info(mes, prop),
                ValidationSeverity.Warning => ValidationMessage.Warn(mes, prop),
                _ => ValidationMessage.Error(mes, prop),
            };
        }

        /// <summary>Returns true if the type implements/inherits from the base type.</summary>
        private static bool Implements(this Type type, Type baseType)
        {
            return baseType.IsAssignableFrom(type);
        }
    }
}
