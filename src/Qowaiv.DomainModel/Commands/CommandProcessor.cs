using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace Qowaiv.DomainModel.Commands
{
    /// <summary>Command processor that resolves the registered command handler
    /// base on the type of the command.
    /// </summary>
    /// <typeparam name="TReturnType">
    /// The return type of the command handler methods
    /// </typeparam>
    /// <remarks>
    /// It compiles (one time per command type) the expression to handle the
    /// command on the command handler.
    /// </remarks>
    public abstract class CommandProcessor<TReturnType>
    {
        /// <summary>The generic type definition of the command handlers to support.</summary>
        /// <remarks>
        /// Something like <code>typeof(CommandHandler&lt;&gt;)</code>.
        /// </remarks>
        protected abstract Type GenericHandlerType { get; }

        /// <summary>The name of the method defined on the generic interface.</summary>
        /// <remarks>
        /// Something like "Handle", or "HandleAsync".
        /// </remarks>
        protected abstract string HandlerMethod { get; }
        
        /// <summary>Gets the command handler to send the command to.</summary>
        /// <param name="handlerType">
        /// The resolved command handler type to get the instance of.
        /// </param>
        /// <remarks>
        /// This is the method that could/should hook on to your DI of choice.
        /// </remarks>
        protected abstract object GetHandler(Type handlerType);

        /// <summary>Sends the command to the registered command handler and
        /// communicates its response.
        /// </summary>
        /// <param name="command">
        /// The command that should be handled by some command handler.
        /// </param>
        /// <returns>
        /// The response of the registered command handler.
        /// </returns>
        public TReturnType Send(object command)
        {
            var commandType = Guard.NotNull(command, nameof(command)).GetType();
            var handlerType = HandlerTypeFor(commandType);
            var handler = GetHandler(handlerType) ?? throw new UnresolvedCommandHandler(handlerType);
            return Handle(handlerType, commandType)(handler, command);
        }

        public TReturnType Send(object command, CancellationToken token)
        {
            var commandType = Guard.NotNull(command, nameof(command)).GetType();
            var handlerType = HandlerTypeFor(commandType);
            var handler = GetHandler(handlerType) ?? throw new UnresolvedCommandHandler(handlerType);
            var method = GetMethodWithToken(handlerType, commandType) ?? throw new InvalidOperationException("Overload does not exist");
            return (TReturnType)method.Invoke(handler, new[] { command, token });
        }


        /// <summary>Gets function to invoke.</summary>
        private Func<object, object, TReturnType> Handle(Type handler, Type command)
        {
            if (!handlers.TryGetValue(command, out var handle))
            {
                handle = GetExpression(GetMethod(handler, command) ?? throw new InvalidOperationException("Overload does not exist")).Compile();
                handlers[command] = handle;
            }
            return handle;
        }

        /// <summary>Gets the generic interface for the specific command handler.</summary>
        /// <param name="commandType">
        /// The type of the command to process.
        /// </param>
        private Type HandlerTypeFor(Type commandType) => GenericHandlerType.MakeGenericType(commandType);

        /// <summary>Gets the <see cref="MethodInfo"/> for the handler method to call.</summary>
        private MethodInfo GetMethod(Type handlerType, Type commandType)
            => handlerType.GetMethods()
                .FirstOrDefault(m => m.Name == HandlerMethod
                    && m.GetParameters().Length == 1
                    && m.GetParameters()[0].ParameterType == commandType);

        /// <summary>Gets the <see cref="MethodInfo"/> for the handler method to call.</summary>
        private MethodInfo GetMethodWithToken(Type handlerType, Type commandType)
            => handlerType.GetMethods()
                .FirstOrDefault(m => m.Name == HandlerMethod
                    && m.GetParameters().Length == 2
                    && m.GetParameters()[0].ParameterType == commandType
                    && m.GetParameters()[1].ParameterType == typeof(CancellationToken));

        /// <summary>Gets an expression that calls the Handle method.</summary>
        /// <remarks>
        /// (handler, cmd) => ((HandlerType)handler).{HandlerMethod}((CommandType)cmd);
        /// </remarks>
        private static Expression<Func<object, object, TReturnType>> GetExpression(MethodInfo method)
        {
            var handler = Expression.Parameter(typeof(object), "handler");
            var cmd = Expression.Parameter(typeof(object), "cmd");
            var typedCommand = Expression.Convert(cmd, method.GetParameters()[0].ParameterType);
            var typedHandler = Expression.Convert(handler, method.DeclaringType);
            var body = Expression.Call(typedHandler, method, typedCommand);
            return Expression.Lambda<Func<object, object, TReturnType>>(body, handler, cmd);
        }

        private readonly Dictionary<Type, Func<object, object, TReturnType>> handlers = new();
    }
}
