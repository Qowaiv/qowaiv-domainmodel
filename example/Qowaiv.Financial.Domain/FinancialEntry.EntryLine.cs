using System;
using System.Collections.Generic;
using System.Text;

namespace Qowaiv.Financial.Domain
{
    public partial class FinancialEntry
    {
        public class EntryLine
        {
            public Money Amount { get; set; }
            public Date Date { get; set; }
            public string Description { get; set; }
            public GlAccountCode GlAccount { get; set; }
            public Guid? AccountId { get; set; }
        }
    }
}

