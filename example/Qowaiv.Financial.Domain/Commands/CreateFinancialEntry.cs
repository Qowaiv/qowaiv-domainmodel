using System;

namespace Qowaiv.Financial.Domain.Commands
{
    public class CreateFinancialEntry : FinancialEntryCommand
    {
        public DivisionCode Division { get; set; }
        public Year ReportingYear { get; set; }
        public Month ReportingMonth { get; set; }
        public FinancialEntryLine[] Lines { get; set; }
    }
}
