using Qowaiv.DomainModel;
using Qowaiv.Validation.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qowaiv.Financial.Shared
{
    public class AggregateRootBase<TAggregate> : AggregateRoot<TAggregate>
        where TAggregate : AggregateRootBase<TAggregate>
    {
        public AggregateRootBase(Guid id, IValidator<TAggregate> validator) : base(validator)
        {
            Buffer = new EventBuffer(id);
        }

        public Guid Id => Buffer.Id;

        public EventBuffer Buffer { get; }


        protected override void AddEventsToStream(params object[] events)
        {
            Buffer.AddRange(events);
        }
    }
}
