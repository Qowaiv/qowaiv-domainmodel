namespace Qowaiv.Financial.Web
{
    /// <summary>The normalized API response.</summary>
    /// <typeparam name="TModel">
    /// The model to return with the API response.
    /// </typeparam>
    public class ApiResponse<TModel> : ApiResponse
    {
        /// <summary>The value of the model.</summary>
        public TModel Value { get; set; }
    }
}
