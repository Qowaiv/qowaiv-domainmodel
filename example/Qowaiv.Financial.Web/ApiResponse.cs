using Qowaiv.Validation.Abstractions;
using System.Collections.Generic;
using System.Linq;

namespace Qowaiv.Financial.Web
{
    /// <summary>The normalized API response.</summary>
    public class ApiResponse
    {
        /// <summary>The error messages (if any).</summary>
        public ValidationMessage[] Errors { get; set; }

        /// <summary>The warning messages (if any).</summary>
        public ValidationMessage[] Warnings { get; set; }

        /// <summary>The info messages (if any).</summary>
        public ValidationMessage[] Infos { get; set; }

        /// <summary>Validation message.</summary>
        public class ValidationMessage
        {
            /// <summary>The validation message.</summary>
            public string Message { get; set; }

            /// <summary>The linked property (if any).</summary>
            public string Property { get; set; }

            internal static ValidationMessage[] Select(IEnumerable<IValidationMessage> messages)
            {
                if (messages is null || !messages.Any())
                {
#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
                    return null;
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null
                }
                return messages.Select(m => new ValidationMessage { Message = m.Message, Property = m.PropertyName }).ToArray();
            }
        }

        /// <summary>Creates an <see cref="ApiResponse{TModel}"/> from the <see cref="Result{TModel}"/>.</summary>
        /// <typeparam name="TModel">
        /// The type of the model.
        /// </typeparam>
        public static ApiResponse<TModel> From<TModel>(Result<TModel> result)
        {
            if (result is null)
            {
                return new ApiResponse<TModel>();
            }

            return new ApiResponse<TModel>
            {
                Value = result.IsValid ? result.Value : default,
                Errors = ValidationMessage.Select(result.Errors),
                Warnings = ValidationMessage.Select(result.Warnings),
                Infos = ValidationMessage.Select(result.Infos),
            };
        }

        /// <summary>Creates an <see cref="ApiResponse"/> from the <see cref="Result"/>.</summary>
        public static ApiResponse From(Result result)
        {
            if (result is null)
            {
                return new ApiResponse();
            }

            return new ApiResponse
            {
                Errors = ValidationMessage.Select(result.Errors),
                Warnings = ValidationMessage.Select(result.Warnings),
                Infos = ValidationMessage.Select(result.Infos),
            };
        }
    }
}
