using GlAccountCode = Qowaiv.Identifiers.Id<Qowaiv.Financial.Shared.ForGlAcount>;
using AccountId = Qowaiv.Identifiers.Id<Qowaiv.Financial.Shared.ForGlAcount>;

namespace Qowaiv.Financial.Domain
{
    public partial class FinancialEntry
    {
        public sealed record EntryLine(
            Money Amount,
            Date Date,
            string Description,
            GlAccountCode GlAccount,
            AccountId AccountId);
    }
}

