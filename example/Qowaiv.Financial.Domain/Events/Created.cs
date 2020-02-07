using System;

namespace Qowaiv.Financial.Domain.Events
{
    public class Created
    {
        public DateTime CreatedUtc { get; set; }
        public Report Report { get;  set; }
    }
}
