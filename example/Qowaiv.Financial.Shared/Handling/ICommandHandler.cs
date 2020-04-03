using Qowaiv.Validation.Abstractions;
using System.Threading.Tasks;

namespace Qowaiv.Financial.Shared.Handling
{
    /// <summary>Generic command handler.</summary>
    /// <typeparam name="TCommand">
    /// The type of the command.
    /// </typeparam>
    public interface ICommandHandler<TCommand> where TCommand : class
    {
        /// <summary>Handles a command asynchronously.</summary>
        /// <param name="command">
        /// The command to sent.
        /// </param>
        /// <returns>
        /// The response result.
        /// </returns>
        Task<Result> HandleAsync(TCommand command);
    }
}
