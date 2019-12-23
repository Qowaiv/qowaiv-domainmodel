using System;
using System.Collections.Generic;
using System.Text;

namespace Qowaiv.Financial.Domain.Events
{
    public class EntryLinesAdded
    {
        public Line[] Lines { get; set; }

        public class Line
        {
            public GlAccountCode GlAccount { get; internal set; }
            public Date Date { get; internal set; }
            public Money Amount { get; internal set; }
            public string Description { get; internal set; }
            public Guid? AccountId { get; internal set; }
        }
    }
}
