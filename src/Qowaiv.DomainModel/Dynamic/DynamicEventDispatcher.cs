using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Qowaiv.DomainModel.Dynamic
{
    /// <summary>A dynamic event dispatcher, is a extremely limited dynamic object
    /// that is capable of invoking instance methods with the signature When(@event).
    /// </summary>
    /// <typeparam name="TDispatcher">
    /// The type of the non dynamic event dispatcher.
    /// </typeparam>
    /// <remarks>
    /// The constraints on the method:
    /// * Name of the method is 'When'
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
            => this.dispatcher = Guard.NotNull(dispatcher, nameof(dispatcher));

        private readonly TDispatcher dispatcher;

        /// <summary>Tries to invoke a (void) When(@event) method.</summary>
        /// <exception cref="EventTypeNotSupported">
        /// If the invoke call was on (void) When(@event) but the type was not available.
        /// </exception>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
            => binder?.Name == nameof(When) && args?.Length == 1 && !(args[0] is null)
            ? When(args[0], out result)
            : base.TryInvokeMember(binder, args, out result);

        /// <summary>Gets the supported event types.</summary>
        public IReadOnlyCollection<Type> SupportedEventTypes => Lookup.Keys;

        /// <summary>Invokes the When(@event) method.</summary>
        private bool When(object @event, out object result)
        {
            var eventType = @event.GetType();
            if (Lookup.TryGetValue(eventType, out var when))
            {
                result = null;
                when(dispatcher, @event);
                return true;
            }
            throw new EventTypeNotSupported(eventType, typeof(TDispatcher));
        }

        /// <summary>Initializes all When(@event) methods.</summary>
        private static Dictionary<Type, Action<TDispatcher, object>> Init()
        {
            var lookup = new Dictionary<Type, Action<TDispatcher, object>>();

            foreach (var method in WhenMethods())
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

        /// <summary>Uses reflection to resolve `When` methods to use,
        /// both non-public (mostly internal) and public.
        /// </summary>
#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
        private static IEnumerable<MethodInfo> WhenMethods()
            => typeof(TDispatcher)
            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(method => method.Name == nameof(When));
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields

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
