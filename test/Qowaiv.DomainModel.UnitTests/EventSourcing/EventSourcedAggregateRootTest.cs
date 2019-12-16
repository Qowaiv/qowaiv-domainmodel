using NUnit.Framework;
using Qowaiv.DomainModel.EventSourcing;
using Qowaiv.DomainModel.TestTools.EventSourcing;
using Qowaiv.DomainModel.UnitTests.Models;
using Qowaiv.Validation.Abstractions;
using Qowaiv.Validation.TestTools;
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
            stream.Add(new NameUpdated());

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

            ValidationMessageAssert.IsValid(aggregate.SetName("Jimi Hendrix"));
            AggregateRootAssert.HasUncommittedEvents(aggregate.EventStream, new NameUpdated { Name = "Jimi Hendrix" });
        }

        [Test]
        public void ApplyEvents_SomeEvents_UpdatesAggregate()
        {
            var aggregate = new SimpleEventSourcedRoot();

            ValidationMessageAssert.IsValid(aggregate.SetPerson("Jimi Hendrix", new Date(1942, 11, 27)));
            AggregateRootAssert.HasUncommittedEvents(aggregate.EventStream, 
                new NameUpdated { Name = "Jimi Hendrix" },
                new DateOfBirthUpdated { DateOfBirth = new Date(1942, 11, 27) });
        }

        [Test]
        public void ApplyEvent_NotSupported()
        {
            var aggregate = new TestApplyChangeAggregate();
            var exception = Assert.Throws<EventTypeNotSupportedException>(() => aggregate.TestApplyChange(new NameUpdated()));
            Assert.AreEqual(typeof(NameUpdated), exception.EventType);
        }


        private class TestApplyChangeAggregate : AggregateRoot<TestApplyChangeAggregate>
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
