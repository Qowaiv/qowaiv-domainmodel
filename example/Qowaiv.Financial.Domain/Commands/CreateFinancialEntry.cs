using DivisionCode = Qowaiv.Identifiers.Id<Qowaiv.Financial.Shared.ForDivision>;
using FinancialEntryId = Qowaiv.Identifiers.Id<Qowaiv.Financial.Shared.ForFinancialEntry>;

namespace Qowaiv.Financial.Domain.Commands
{
    public record CreateFinancialEntry(
        DivisionCode Division,
        Year ReportingYear,
        Month ReportingMonth,
        FinancialEntryLine[] Lines) : FinancialEntryCommand(FinancialEntryId.Next());
 }
