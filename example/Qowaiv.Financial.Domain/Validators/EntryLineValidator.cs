using FluentValidation;
using Qowaiv.Validation.Fluent;
using static Qowaiv.Financial.Domain.FinancialEntry;

namespace Qowaiv.Financial.Domain.Validators
{
    public class EntryLineValidator : FluentModelValidator<EntryLine>
    {
        public EntryLineValidator()
        {
            RuleFor(line => line.Date).NotEmpty();
            RuleFor(line => line.GlAccount).NotEmpty();
            RuleFor(line => line.Description).NotEmpty();
        }
    }
}
