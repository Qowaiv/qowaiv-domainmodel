using Qowaiv.Validation.Abstractions;

namespace ConquerClub.Domain.Validation
{
    internal static class Must
    {
        public static Result<T> BeTrue<T>(T game, bool condition, string message, params object[] args)
            => condition
                ? Result.For(game)
                : Result.For(game, ValidationMessage.Error(string.Format(message, args)));
    }
}
