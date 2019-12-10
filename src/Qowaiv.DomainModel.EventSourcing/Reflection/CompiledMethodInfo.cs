﻿using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace Qowaiv.DomainModel.EventSourcing.Reflection
{
    /// <summary>An implementation of <see cref="MethodInfo"/> that executes
    /// <see cref="MethodBase.Invoke(object, BindingFlags, Binder, object[], CultureInfo)"/>
    /// by executing a (compiled) <see cref="Func{T1, T2, TResult}"/>.
    /// </summary>
    internal class CompiledMethodInfo : MethodInfo
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly MethodInfo _method;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Func<object, object[], object> _func;

        /// <summary>Initializes a new instance of the <see cref="CompiledMethodInfo"/> class.</summary>
        /// <param name="methodInfo">
        /// The underlying method info.
        /// </param>
        public CompiledMethodInfo(MethodInfo methodInfo)
        {
            _method = Guard.NotNull(methodInfo, nameof(methodInfo));
            _func = Compile(methodInfo);
        }

        /// <inheritdoc />
        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
        {
            return _func(obj, parameters);
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string Name => _method.Name;

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override Type DeclaringType => _method.DeclaringType;

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override Type ReflectedType => _method.ReflectedType;

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override RuntimeMethodHandle MethodHandle => _method.MethodHandle;

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override MethodAttributes Attributes => _method.Attributes;

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override ICustomAttributeProvider ReturnTypeCustomAttributes => _method.ReturnTypeCustomAttributes;

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override MethodInfo GetBaseDefinition() => _method.GetBaseDefinition();

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override object[] GetCustomAttributes(bool inherit) => _method.GetCustomAttributes(inherit);

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override object[] GetCustomAttributes(Type attributeType, bool inherit) => _method.GetCustomAttributes(attributeType, inherit);

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override MethodImplAttributes GetMethodImplementationFlags() => _method.GetMethodImplementationFlags();

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override ParameterInfo[] GetParameters() => _method.GetParameters();

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override bool IsDefined(Type attributeType, bool inherit) => _method.IsDefined(attributeType, inherit);

        /// <summary>Compiles a <see cref="Func{T1, T2, TResult}"/> based on the method info.</summary>
        private static Func<object, object[], object> Compile(MethodInfo methodInfo)
        {
            var instanceExpression = Expression.Parameter(typeof(object), "instance");
            var argumentsExpression = Expression.Parameter(typeof(object[]), "arguments");
            var parameterInfos = methodInfo.GetParameters();

            var argumentExpressions = new Expression[parameterInfos.Length];
            for (var i = 0; i < parameterInfos.Length; ++i)
            {
                var parameterInfo = parameterInfos[i];
                argumentExpressions[i] = Expression.Convert(Expression.ArrayIndex(argumentsExpression, Expression.Constant(i)), parameterInfo.ParameterType);
            }
            var callExpression = Expression.Call(!methodInfo.IsStatic ? Expression.Convert(instanceExpression, methodInfo.ReflectedType) : null, methodInfo, argumentExpressions);
            if (callExpression.Type == typeof(void))
            {
                var action = Expression.Lambda<Action<object, object[]>>(callExpression, instanceExpression, argumentsExpression).Compile();
                return (instance, arguments) =>
                {
                    action(instance, arguments);
                    return null;
                };
            }
            return Expression.Lambda<Func<object, object[], object>>(Expression.Convert(callExpression, typeof(object)), instanceExpression, argumentsExpression).Compile();
        }
    }
}
