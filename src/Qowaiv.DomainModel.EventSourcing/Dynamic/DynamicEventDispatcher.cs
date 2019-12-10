﻿using Qowaiv.DomainModel.EventSourcing.Reflection;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
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
                result = Apply(args);
                return true;
            }
            return base.TryInvokeMember(binder, args, out result);
        }

        /// <summary>Gets the supported event types.</summary>
        public IReadOnlyCollection<Type> SupportedEventTypes => Lookup[objectType].Keys;

        /// <summary>Invokes the Apply(@event) method.</summary>
        private object Apply(object[] args)
        {
            var eventType = args[0].GetType();
            if (Lookup[objectType].TryGetValue(eventType, out MethodInfo apply))
            {
                return apply.Invoke(@object, args);
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
                        var cache = new Dictionary<Type, MethodInfo>();

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

                                // Leave out object itself, primitives and enums.
                                if (parameterType != typeof(object) &&
                                    !parameterType.IsPrimitive &&
                                    !parameterType.IsEnum)
                                {
                                    cache[parameterType] = new CompiledMethodInfo(method);
                                }
                            }
                        }
                        Lookup[objectType] = cache;
                    }
                }
            }
        }

        private static readonly object Locker = new object();
        private static readonly Dictionary<Type, Dictionary<Type, MethodInfo>> Lookup = new Dictionary<Type, Dictionary<Type, MethodInfo>>();
    }
}
