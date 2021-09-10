using AccountId = Qowaiv.Identifiers.Id<Qowaiv.Financial.Shared.ForGlAcount>;
using GlAccountCode = Qowaiv.Identifiers.Id<Qowaiv.Financial.Shared.ForGlAcount>;

namespace Qowaiv.Financial.Domain.Commands
{
    public record FinancialEntryLine(
        Money Amount,
        Date Date,
        string Description,
        GlAccountCode GlAccount,
        AccountId AccountId);
}
