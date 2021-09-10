using FinancialEntryId = Qowaiv.Identifiers.Id<Qowaiv.Financial.Shared.ForFinancialEntry>;

namespace Qowaiv.Financial.Domain.Commands
{
    public record FinancialEntryCommand(FinancialEntryId Id);
}
