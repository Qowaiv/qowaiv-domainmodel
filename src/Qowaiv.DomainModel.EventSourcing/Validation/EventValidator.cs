using Qowaiv.Validation.Abstractions;
using System;
using System.Linq;
using System.Reflection;

namespace Qowaiv.DomainModel.EventSourcing.Validation
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
            where TAggregate : AggregateRoot<TAggregate>
        {
            var eventValidatorType = GetValidatorType(typeof(TAggregate), @event.GetType());
            var validatorProperty = GetValidatorProperty(validator.GetType(), eventValidatorType);

            if (validatorProperty is null)
            {
                return aggregate;
            }

            var context = CreateContext(aggregate, @event);
            var eventValidator = validatorProperty.GetValue(validator);
            var validate = eventValidatorType.GetMethod(Validate);
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

        /// <summary>Gets The validator type <see cref="IValidator{TModel}"/> where <code>TModel</code>
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

        /// <summary>Gets the validator property.</summary>
        private static PropertyInfo GetValidatorProperty(Type validatorType, Type eventValidatorType)
        {
            return validatorType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(property => Implements(property, eventValidatorType));
        }

        /// <summary>Returns true if the property type implements/inherits from the base type.</summary>
        private static bool Implements(PropertyInfo property, Type baseType)
        {
            return baseType.IsAssignableFrom(property.PropertyType);
        }
    }
}
