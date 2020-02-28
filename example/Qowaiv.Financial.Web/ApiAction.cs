using Microsoft.AspNetCore.Mvc;
using Qowaiv.Validation.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qowaiv.Financial.Web
{
    /// <summary>API action handler.</summary>
    public static class ApiAction
    {
        /// <summary>Creates a HTTP GET response.</summary>
        /// <param name="promise">
        /// The result potentially containing error messages).
        /// </param>
        /// <typeparam name="TModel">
        /// The model to return.
        /// </typeparam>
        public static async Task<IActionResult> GetAsync<TModel>(Task<Result<TModel>> promise) where TModel : class
        {
            _ = Guard.NotNull(promise, nameof(promise));

            var result = await promise;

            if (IsError(result, out IActionResult error))
            {
                return error;
            }
            return new OkObjectResult(ApiResponse.From(result));
        }

        /// <summary>Creates a HTTP GET response.</summary>
        /// <param name="promise">
        /// The result potentially containing error messages).
        /// </param>
        /// <typeparam name="TModel">
        /// The model to return.
        /// </typeparam>
        public static IActionResult Get<TModel>(Result<TModel> promise)
        {
            _ = Guard.NotNull(promise, nameof(promise));

            var result = promise;

            if (IsError(result, out IActionResult error))
            {
                return error;
            }
            return new OkObjectResult(ApiResponse.From(result));
        }

        /// <summary>Creates a HTTP GET response for POST calls that for security or practical reasons cannot be called as GET.</summary>
        /// <param name="promise">
        /// The result potentially containing error messages.
        /// </param>
        /// <typeparam name="TModel">
        /// The model to return.
        /// </typeparam>
        public static Task<IActionResult> GetAsPostAsync<TModel>(Task<Result<TModel>> promise) where TModel : class
        {
            return GetAsync(promise);
        }

        /// <summary>Creates a HTTP GET response for POST calls that for security or practical reasons cannot be called as GET.</summary>
        /// <param name="promise">
        /// The result potentially containing error messages.
        /// </param>
        /// <typeparam name="TModel">
        /// The model to return.
        /// </typeparam>
        public static IActionResult GetAsPost<TModel>(Result<TModel> promise)
        {
            return Get(promise);
        }

        /// <summary>Creates a HTTP POST response.</summary>
        /// <param name="promise">
        /// The result potentially containing error messages.
        /// </param>
        /// <param name="location">
        /// The location to navigate to.
        /// </param>
        public static async Task<IActionResult> PostAsync(Task<Result> promise, string location)
        {
            _ = Guard.NotNull(promise, nameof(promise));

            var result = await promise;

            if (IsError(result, out IActionResult error))
            {
                return error;
            }
            return new CreatedResult(location, null);
        }

        /// <summary>Creates a HTTP PUT response.</summary>
        /// <param name="promise">
        /// The result potentially containing error messages.
        /// </param>
        public static async Task<IActionResult> PutAsync(Task<Result> promise)
        {
            _ = Guard.NotNull(promise, nameof(promise));

            var result = await promise;

            if (IsError(result, out IActionResult error))
            {
                return error;
            }
            if (result.Messages.Any())
            {
                return new OkObjectResult(ApiResponse.From(result));
            }
            return new NoContentResult();
        }

        /// <summary>Checks if the result IsValid, if not, the out parameter
        /// represents the error response.
        /// </summary>
        private static bool IsError(Result result, out IActionResult errorResult)
        {
            if (result is null)
            {
                errorResult = new NotFoundResult();
                return true;
            }
            if (!result.IsValid)
            {
                var data = ApiResponse.From(result);
                errorResult = new UnprocessableEntityObjectResult(data);
                return true;
            }
            errorResult = null;
            return false;
        }
    }
}
