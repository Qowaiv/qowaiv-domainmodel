using FluentValidation;
using Qowaiv.Validation.Fluent;

namespace ConquerClub.Domain.Validation
{
    internal class CountryValidator : FluentModelValidator<Country>
    {
        public CountryValidator()
        {
            RuleFor(c => c.Army).NotEmpty();
        }
    }
}
