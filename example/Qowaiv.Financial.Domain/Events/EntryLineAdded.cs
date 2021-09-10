using GlAccountCode = Qowaiv.Identifiers.Id<Qowaiv.Financial.Shared.ForGlAcount>;
using AccountId = Qowaiv.Identifiers.Id<Qowaiv.Financial.Shared.ForGlAcount>;

namespace Qowaiv.Financial.Domain.Events
{
    public record EntryLineAdded(
        GlAccountCode GlAccount,
        Date Date,
        Money Amount,
        string Description,
        AccountId AccountId);
}
