using System;

namespace Qowaiv.Financial.Domain.Events
{
    public class Created
    {
        public DateTime CreatedUtc { get; set; }
        public Report Report { get;  set; }
        public Line[] Lines { get;  set; }

        public class Line
        {
            public GlAccountCode GlAccount { get;  set; }
            public Date Date { get;  set; }
            public Money Amount { get;  set; }
            public string Description { get;  set; }
            public Guid? AccountId { get;  set; }
        }
    }
}
