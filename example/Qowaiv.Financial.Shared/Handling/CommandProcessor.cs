using Qowaiv.Validation.Abstractions;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Qowaiv.Financial.Shared.Handling
{
    /// <summary>Generic command processor.</summary>
    public class CommandProcessor
    {
        /// <summary>Initializes a new instance of the <see cref="CommandProcessor"/> class.</summary>
        public CommandProcessor(IServiceProvider provider)
        {
            this.provider = Guard.NotNull(provider, nameof(provider));
        }

        private readonly IServiceProvider provider;

        /// <summary>Sends the command asynchronously.</summary>
        /// <param name="command">
        /// The command to sent.
        /// </param>
        /// <typeparam name="TCommand">
        /// The type of the command.
        /// </typeparam>
        /// <returns>
        /// The response given by the triggered <see cref="ICommandHandler{TCommand}"/>.
        /// </returns>
        public Task<Result> SendAsync<TCommand>(TCommand command)
            where TCommand : class
        {
            Guard.NotNull(command, nameof(command));
            var commandType = command.GetType();
            var handlerType = GetCommandHandlerType(commandType);

            var handler = provider.GetService(handlerType);

            if (handler is null)
            {
                throw new InvalidOperationException($"Could not resolve the handler of the type ICommandHandler<{commandType}>.");
            }

            var handleAsync = HandleAsync(handlerType, commandType);

            var result = (Task<Result>)handleAsync.Invoke(handler, new[] { command });

            return result;
        }

        private static Type GetCommandHandlerType(Type commandType)
        {
            Guard.NotNull(commandType, nameof(commandType));
            var tp = typeof(ICommandHandler<>);
            var genereric = tp.MakeGenericType(commandType);
            return genereric;
        }

        private static MethodInfo HandleAsync(Type handlerType, Type commandType)
        {
            var method = handlerType.GetMethods()
                .FirstOrDefault(m => m.Name == nameof(ICommandHandler<object>.HandleAsync)
                    && m.GetParameters().Length == 1
                    && m.GetParameters()[0].ParameterType == commandType);
            return method;
        }
    }
}
