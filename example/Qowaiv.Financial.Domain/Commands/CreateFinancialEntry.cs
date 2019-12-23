using System;
using System.Collections.Generic;
using System.Text;

namespace Qowaiv.Financial.Domain.Commands
{
    public class CreateFinancialEntry
    {
        public Guid Id { get; set; }
        public Year ReportingYear { get; set; }
        public Month ReportingMonth { get; set; }
        public FinancialEntryLine[] Lines { get; set; }
    }
}
