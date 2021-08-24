using NUnit.Framework;
using Qowaiv.DomainModel.TestTools.EventSourcing;
using Qowaiv.DomainModel.UnitTests.Models;
using Qowaiv.Validation.Abstractions;
using Qowaiv.Validation.TestTools;
using System;

namespace Qowaiv.DomainModel.UnitTests
{
    public class EventSourcedAggregateRootTest
    {
        [Test]
        public void Ctor_NoParameters_SetsId()
        {
            var aggregate = new SimpleEventSourcedRoot();

            Assert.AreNotEqual(Guid.Empty, aggregate.Id);
            Assert.AreEqual(aggregate.Id, aggregate.Buffer.AggregateId);
            Assert.AreEqual(0, aggregate.Version);
        }

        [Test]
        public void ApplyEvent_SomeEvent_UpdatesAggregate()
        {
            var aggregate = new SimpleEventSourcedRoot();

            var updated = ValidationMessageAssert.IsValid(aggregate.SetName("Jimi Hendrix"));
            AggregateRootAssert.HasUncommittedEvents(updated.Buffer, new NameUpdated { Name = "Jimi Hendrix" });
        }

        [Test]
        public void ApplyEvents_SomeEvents_UpdatesAggregate()
        {
            var aggregate = new SimpleEventSourcedRoot();

            var updated = ValidationMessageAssert.IsValid(aggregate.SetPerson("Jimi Hendrix", new Date(1942, 11, 27)));
            AggregateRootAssert.HasUncommittedEvents(updated.Buffer,
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


        private class TestApplyChangeAggregate : AggregateRoot<TestApplyChangeAggregate, Guid>
        {
            public TestApplyChangeAggregate()
                : base(Guid.NewGuid(), Qowaiv.Validation.Abstractions.Validator.Empty<TestApplyChangeAggregate>()) { }

            public void TestApplyChange(object @event)
            {
                ApplyEvent(@event);
            }
        }
    }
}
