using System;

namespace Qowaiv.Financial.Domain.Events
{
    public class EntryLineAdded
    {
        public GlAccountCode GlAccount { get; set; }
        public Date Date { get; set; }
        public Money Amount { get; set; }
        public string Description { get; set; }
        public Guid? AccountId { get; set; }
    }
}
