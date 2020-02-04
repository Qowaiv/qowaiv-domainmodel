using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Qowaiv.DomainModel.Dynamic
{
    /// <summary>A dynamic event dispatcher, is a extremely limited dynamic object
    /// that is capable of invoking instance methods with the signature Apply(@event).
    /// </summary>
    /// <typeparam name="TDispatcher">
    /// The type of the non dynamic event dispatcher.
    /// </typeparam>
    /// <remarks>
    /// The constraints on the method:
    /// * Name of the method is 'Apply'
    /// * Binding is on instance (both public and non-public)
    /// * One parameter with a type that is/could be an event.
    /// * Return type is ignored.
    ///
    /// It caches the available methods per type.
    /// </remarks>
    public class DynamicEventDispatcher<TDispatcher> : DynamicObject
        where TDispatcher : class
    {
        /// <summary>Initializes a new instance of the <see cref="DynamicEventDispatcher{TDispatcher}"/> class..</summary>
        public DynamicEventDispatcher(TDispatcher dispatcher)
        {
            this.dispatcher = Guard.NotNull(dispatcher, nameof(dispatcher));
        }

        private readonly TDispatcher dispatcher;

        /// <summary>Tries to invoke a (void) Apply(@event) method.</summary>
        /// <exception cref="EventTypeNotSupportedException">
        /// If the invoke call was on (void) Apply(@event) but the type was not available.
        /// </exception>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (binder?.Name == nameof(Apply) && args?.Length == 1)
            {
                result = Apply(args[0]);
                return true;
            }
            return base.TryInvokeMember(binder, args, out result);
        }

        /// <summary>Gets the supported event types.</summary>
        public IReadOnlyCollection<Type> SupportedEventTypes => Lookup.Keys;

        /// <summary>Invokes the Apply(@event) method.</summary>
        private object Apply(object @event)
        {
            var eventType = @event.GetType();
            if (Lookup.TryGetValue(eventType, out var apply))
            {
                apply(dispatcher, @event);
                return null;
            }
            throw new EventTypeNotSupportedException(eventType, typeof(TDispatcher));
        }

        /// <summary>Initializes all Apply(@event) methods.</summary>
        private static Dictionary<Type, Action<TDispatcher, object>> Init()
        {
            var lookup = new Dictionary<Type, Action<TDispatcher, object>>();

            const string name = nameof(Apply);
            var methods = typeof(TDispatcher)
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(method => method.Name == name);

            foreach (var method in methods)
            {
                var parameters = method.GetParameters();
                if (parameters.Length == 1)
                {
                    var parameterType = parameters[0].ParameterType;

                    // Leave out object itself, primitives and enumerations.
                    if (parameterType != typeof(object) &&
                        !parameterType.IsPrimitive &&
                        !parameterType.IsEnum)
                    {
                        lookup[parameterType] = Compile(method, parameterType);
                    }
                }
            }
            return lookup;
        }

        private static Action<TDispatcher, object> Compile(MethodInfo method, Type eventType)
        {
            var dispatcherParam = Expression.Parameter(typeof(TDispatcher), "dispatcher");
            var @event = Expression.Parameter(typeof(object), "e");

            var typedEvent = Expression.Convert(@event, eventType);
            var body = Expression.Call(dispatcherParam, method, typedEvent);

            var expression = Expression.Lambda<Action<TDispatcher, object>>(body, dispatcherParam, @event);

            return expression.Compile();
        }

        private static readonly Dictionary<Type, Action<TDispatcher, object>> Lookup = Init();
    }
}
