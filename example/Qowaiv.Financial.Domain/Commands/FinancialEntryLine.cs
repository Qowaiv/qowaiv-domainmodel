using System;

namespace Qowaiv.Financial.Domain.Commands
{
    public class FinancialEntryLine
    {
        public Money Amount { get; set; }
        public Date Date { get; set; }
        public string Description { get; set; }
        public GlAccountCode GlAccount { get; set; }
        public Guid? AccountId { get; set; }
    }
}
