using Qowaiv.Validation.Abstractions;
using System.Threading.Tasks;

namespace Qowaiv.Financial.Shared.Handling
{
    /// <summary>Generic request handler.</summary>
    /// <typeparam name="TRequest">
    /// The type of the request.
    /// </typeparam>
    /// <typeparam name="TResponse">
    /// The type of the response.
    /// </typeparam>
    public interface IRequestHandler<TRequest, TResponse>
    {
        /// <summary>Handles a request asynchronously.</summary>
        /// <param name="request">
        /// The request to sent.
        /// </param>
        /// <returns>
        /// The response result.
        /// </returns>
        Task<Result<TResponse>> HandleAsync(TRequest request);
    }
}
