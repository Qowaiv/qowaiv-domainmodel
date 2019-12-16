using NUnit.Framework;
using Qowaiv.DomainModel.EventSourcing;
using Qowaiv.DomainModel.UnitTests.Models;
using Qowaiv.Validation.Abstractions;
using System;

namespace Qowaiv.DomainModel.UnitTests.EventSourcing
{
    public class EventSourcedAggregateRootTest
    {
        [Test]
        public void FromEvents_NoFullHistory_Throws()
        {
            var stream = new EventStream();
            stream.Add(new SimpleInitEvent());
            stream.MarkAllAsCommitted();
            stream.Add(new UpdateNameEvent());

            Assert.Throws<EventStreamNoFullHistoryException>(() => AggregateRoot.FromEvents<SimpleEventSourcedRoot>(stream));
        }

        [Test]
        public void Ctor_NoParameters_SetsId()
        {
            var aggregate = new SimpleEventSourcedRoot();

            Assert.AreNotEqual(Guid.Empty, aggregate.Id);
            Assert.AreEqual(aggregate.Id, aggregate.EventStream.AggregateId);
            Assert.AreEqual(0, aggregate.Version);
        }

        [Test]
        public void ApplyEvent_SomeEvent_UpdatesAggregate()
        {
            var aggregate = new SimpleEventSourcedRoot();
            aggregate = aggregate.SetName(new UpdateNameEvent { Name = "Nelis Bijl" }).Value;

            Assert.AreEqual("Nelis Bijl", aggregate.Name);
            Assert.AreEqual(0, aggregate.EventStream.CommittedVersion);
            Assert.AreEqual(1, aggregate.Version);
        }

        [Test]
        public void ApplyEvent_NotSupported()
        {
            var aggregate = new TestApplyChangeAggregate();
            var exception = Assert.Throws<EventTypeNotSupportedException>(() => aggregate.TestApplyChange(new UpdateNameEvent()));
            Assert.AreEqual(typeof(UpdateNameEvent), exception.EventType);
        }


        private class TestApplyChangeAggregate : EventSourcedAggregateRoot<TestApplyChangeAggregate>
        {
            public TestApplyChangeAggregate()
                : base(Guid.NewGuid(), Validator.Empty<TestApplyChangeAggregate>()) { }

            public void TestApplyChange(object @event)
            {
                ApplyEvent(@event);
            }
        }
    }
}
