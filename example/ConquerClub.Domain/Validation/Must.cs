using Qowaiv.Validation.Abstractions;

namespace ConquerClub.Domain.Validation
{
    internal static class Must
    {
        public static Result<T> NotBe<T>(T model, bool condition, string message, params object[] args)
            => Be(model, !condition, message, args);

        public static Result<T> Be<T>(T model, bool condition, string message, params object[] args)
            => condition
                ? Result.For(model)
                : Result.WithMessages<T>(ValidationMessage.Error(string.Format(message, args)));
    }
}
