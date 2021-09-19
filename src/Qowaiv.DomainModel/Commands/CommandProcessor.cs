using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

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
        public TReturnType Send(object command) => Send(command, token: default);

        /// <summary>Sends the command to the registered command handler and
        /// communicates its response.
        /// </summary>
        /// <param name="command">
        /// The command that should be handled by some command handler.
        /// </param>
        /// <param name="token">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The response of the registered command handler.
        /// </returns>
        public TReturnType Send(object command, CancellationToken token)
        {
            if (!IsReturnTypeAwaitable && token.CanBeCanceled) throw new InvalidOperationException("A CancellationToken was provided, but the result is not awaitable (Task<>) and thus not cancellable");

            var commandType = Guard.NotNull(command, nameof(command)).GetType();
            var handlerType = HandlerTypeFor(commandType);
            var handler = GetHandler(handlerType) ?? throw new UnresolvedCommandHandler(handlerType);
            return Handle(handlerType, commandType)(handler, command, token);
        }

        private bool IsReturnTypeAwaitable => typeof(TReturnType).IsAssignableFrom(typeof(Task<>));

        /// <summary>Gets function to invoke.</summary>
        private Func<object, object, CancellationToken, TReturnType> Handle(Type handler, Type command)
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
                .Where(m => m.Name == HandlerMethod
                    && (m.GetParameters().Length == 1 || HasCancelationToken(m))
                    && m.GetParameters()[0].ParameterType == commandType)
                .OrderByDescending(m => m.GetParameters().Length)
                .FirstOrDefault();

        private static bool HasCancelationToken(MethodInfo method)
            => method.GetParameters().Length == 2 && method.GetParameters()[1].ParameterType == typeof(CancellationToken);

        /// <summary>Gets an expression that calls the Handle method.</summary>
        /// <remarks>
        /// (handler, cmd, token) => ((HandlerType)handler).{HandlerMethod}((CommandType)cmd, token);
        /// 
        /// or if the token can not be consumed by the handler
        /// 
        /// (handler, cmd, token) => ((HandlerType)handler).{HandlerMethod}((CommandType)cmd);
        /// </remarks>
        private static Expression<Func<object, object, CancellationToken, TReturnType>> GetExpression(MethodInfo method)
        {
            var handler = Expression.Parameter(typeof(object), "handler");
            var cmd = Expression.Parameter(typeof(object), "cmd");
            var token = Expression.Parameter(typeof(CancellationToken), "token");
            var typedCommand = Expression.Convert(cmd, method.GetParameters()[0].ParameterType);
            var typedHandler = Expression.Convert(handler, method.DeclaringType);
            var body = method.GetParameters().Length == 1
                ? Expression.Call(typedHandler, method, typedCommand)
                : Expression.Call(typedHandler, method, typedCommand, token);

            return Expression.Lambda<Func<object, object, CancellationToken, TReturnType>>(body, handler, cmd, token);
        }

        private readonly Dictionary<Type, Func<object, object, CancellationToken, TReturnType>> handlers = new();
    }
}
