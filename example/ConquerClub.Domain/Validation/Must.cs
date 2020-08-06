using Qowaiv.Validation.Abstractions;

namespace ConquerClub.Domain.Validation
{
    internal static class Must
    {
        public static Result<T> BeTrue<T>(T model, bool condition, string message, params object[] args)
            => condition
                ? Result.For(model)
                : Result.WithMessages<T>(ValidationMessage.Error(string.Format(message, args)));
    }
}
