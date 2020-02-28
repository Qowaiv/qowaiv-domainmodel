using Qowaiv.Validation.Abstractions;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Qowaiv.Financial.Shared.Handling
{
    /// <summary>Generic request processor.</summary>
    public class RequestProcessor
    {
        /// <summary>Initializes a new instance of the <see cref="RequestProcessor"/> class.</summary>
        public RequestProcessor(IServiceProvider provider)
        {
            serviceProvider = Guard.NotNull(provider, nameof(provider));
        }

        private readonly IServiceProvider serviceProvider;

        /// <summary>Sends the request asynchronously.</summary>
        /// <param name="request">
        /// The request to sent.
        /// </param>
        /// <typeparam name="TRequest">
        /// The type of the request.
        /// </typeparam>
        /// <typeparam name="TResponse">
        /// The type of the expected response.
        /// </typeparam>
        /// <returns>
        /// The response given by the triggered <see cref="IRequestHandler{TRequest, TResponse}"/>.
        /// </returns>
        public Task<Result<TResponse>> SendAsync<TRequest, TResponse>(TRequest request)
        {
            var handlerType = GetRequestHandlerType(typeof(TRequest), typeof(TResponse));

            var handler = serviceProvider.GetService(handlerType);

            if (handler is null)
            {
                throw new InvalidOperationException($"Could not resolve the handler of the type IRequestHandler<{typeof(TRequest)}, {typeof(TResponse)}>.");
            }

            var handleAsync = HandleAsync(handlerType, typeof(TRequest));
            var parameters = new object[] { request };

            return (Task<Result<TResponse>>)handleAsync.Invoke(handler, parameters);
        }

        private static Type GetRequestHandlerType(Type requestType, Type responseType)
        {
            Guard.NotNull(requestType, nameof(requestType));
            var tp = typeof(IRequestHandler<,>);
            var genereric = tp.MakeGenericType(requestType, responseType);
            return genereric;
        }

        private static MethodInfo HandleAsync(Type handlerType, Type requestType)
        {
            var method = handlerType.GetMethods()
                .FirstOrDefault(m => m.Name == nameof(HandleAsync)
                    && m.GetParameters().Length == 1
                    && m.GetParameters()[0].ParameterType == requestType);
            return method;
        }
    }
}
