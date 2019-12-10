using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Qowaiv.DomainModel.EventSourcing.Dynamic
{
    /// <summary>A dynamic event dispatcher, is a extremely limited dynamic object
    /// that is capable of invoking instance methods with the signature Apply(@event).
    /// </summary>
    /// <remarks>
    /// The constraints on the method:
    /// * Name of the method is 'Apply'
    /// * Binding is on instance (both public and non-public)
    /// * One parameter with a type that is/could be an event.
    /// * Return type is ignored.
    ///
    /// It caches the available methods per type.
    /// </remarks>
    internal class DynamicEventDispatcher : DynamicObject
    {
        /// <summary>Initializes a new instance of the <see cref="DynamicEventDispatcher"/> class.Cr.</summary>
        public DynamicEventDispatcher(object obj)
        {
            @object = Guard.NotNull(obj, nameof(obj));
            objectType = obj.GetType();
            InitApplyMethods();
        }

        private readonly object @object;
        private readonly Type objectType;

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
        public IReadOnlyCollection<Type> SupportedEventTypes => Lookup[objectType].Keys;

        /// <summary>Invokes the Apply(@event) method.</summary>
        private object Apply(object @event)
        {
            var eventType = @event.GetType();
            if (Lookup[objectType].TryGetValue(eventType, out Action<object, object> apply))
            {
                apply(@object, @event);
                return null;
            }
            throw new EventTypeNotSupportedException(eventType, objectType);
        }

        /// <summary>Initializes all Apply(@event) methods.</summary>
        private void InitApplyMethods()
        {
            if (!Lookup.ContainsKey(objectType))
            {
                lock (Locker)
                {
                    if (!Lookup.ContainsKey(objectType))
                    {
                        var cache = new Dictionary<Type, Action<object, object>>();

                        const string name = nameof(Apply);
                        var methods = objectType
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
                                    cache[parameterType] = Compile(method, objectType, parameterType);
                                }
                            }
                        }
                        Lookup[objectType] = cache;
                    }
                }
            }
        }

        private Action<object, object> Compile(MethodInfo method, Type dispatherType, Type eventType)
        {
            var dispatcher = Expression.Parameter(typeof(object), "dispatcher");
            var @event = Expression.Parameter(typeof(object), "e");

            var typedDispatcher = Expression.Convert(dispatcher, dispatherType);
            var typedEvent = Expression.Convert(@event, eventType);
            var body = Expression.Call(typedDispatcher, method, typedEvent);

            var expression = Expression.Lambda<Action<object, object>>(body, dispatcher, @event);

            return expression.Compile();
        }

        private static readonly object Locker = new object();
        private static readonly Dictionary<Type, Dictionary<Type, Action<object, object>>> Lookup = new Dictionary<Type, Dictionary<Type, Action<object, object>>>();
    }
}
