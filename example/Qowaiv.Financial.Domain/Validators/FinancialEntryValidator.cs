using FluentValidation;
using FluentValidation.Validators;
using Qowaiv.Validation.Fluent;
using System.Collections.Generic;
using System.Linq;
using static Qowaiv.Financial.Domain.FinancialEntry;

namespace Qowaiv.Financial.Domain.Validators
{
    public class FinancialEntryValidator : FluentModelValidator<FinancialEntry>
    {
        public FinancialEntryValidator()
        {
            RuleFor(entry => entry.Lines).NotEmpty().Custom(BeBalanced);
            RuleFor(entry => entry.Report).NotEmpty();
            RuleForEach(entry => entry.Lines).SetValidator(new EntryLineValidator());
        }

        private void BeBalanced(IReadOnlyCollection<EntryLine> lines, CustomContext context)
        {
            if (lines.Select(line => line.Amount.Currency).Distinct().Count() > 1)
            {
                context.AddFailure(nameof(FinancialEntry.Lines), "Multiple currencies.");
                // return as we can not sum the amounts.
                return;
            }
            var sum = lines.Select(line => line.Amount).Sum();
            if (sum.Currency.IsEmptyOrUnknown())
            {
                context.AddFailure(nameof(FinancialEntry.Lines), "Unknown currency.");
            }

            if (sum.Amount != Amount.Zero)
            {
                context.AddFailure(nameof(FinancialEntry.Lines), "The lines are note balanced.");
            }
        }
    }
}
