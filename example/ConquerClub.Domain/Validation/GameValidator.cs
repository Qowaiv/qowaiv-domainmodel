using FluentValidation;
using Qowaiv.Validation.Fluent;

namespace ConquerClub.Domain.Validation
{
    internal class GameValidator : FluentModelValidator<Game>
    {
        public GameValidator()
        {
            RuleFor(g => g.Settings).Required();
            RuleFor(g => g.Countries).Required();
            RuleFor(g => g.Phase).NotEmpty();
            RuleFor(g => g.Round).LessThanOrEqualTo(game => game.Settings.RoundLimit);

            RuleForEach(g => g.Countries).SetValidator(new CountryValidator());
        }
    }
}
