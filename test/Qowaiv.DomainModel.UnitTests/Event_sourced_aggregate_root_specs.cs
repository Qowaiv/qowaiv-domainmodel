using NUnit.Framework;
using Qowaiv;
using Qowaiv.DomainModel;
using Qowaiv.DomainModel.TestTools.EventSourcing;
using Qowaiv.DomainModel.UnitTests.Models;
using Qowaiv.Validation.TestTools;
using System;
using System.Linq;

namespace Event_sourced_aggregate_root_specs
{
    public class Applies_events
    {
        [Test]
        public void changes_for_single_event()
        {
            var origin = new SimpleEventSourcedRoot();
            var updated = ValidationMessageAssert.IsValid(origin.SetName("Jimi Hendrix"));
            AggregateRootAssert.HasUncommittedEvents(updated.Buffer, new NameUpdated { Name = "Jimi Hendrix" });
            Assert.That(updated.Name, Is.EqualTo("Jimi Hendrix"));
        }

        [Test]
        public void changes_for_multiple_events()
        {
            var origin = new SimpleEventSourcedRoot();
            var updated = ValidationMessageAssert.IsValid(origin.SetPerson("Jimi Hendrix", new Date(1942, 11, 27)));
            AggregateRootAssert.HasUncommittedEvents(updated.Buffer,
                new NameUpdated { Name = "Jimi Hendrix" },
                new DateOfBirthUpdated { DateOfBirth = new Date(1942, 11, 27) });
        }


        [Test]
        public void as_uncommitted_to_the_buffer()
        {
            var origin = new SimpleEventSourcedRoot();
            var updated = ValidationMessageAssert.IsValid(origin.SetName("Jimi Hendrix"));
            Assert.That(updated.Buffer.Uncommitted.ToList(), Has.Count.EqualTo(1));
        }

        [Test]
        public void with_origin_unchanged()
        {
            var origin = new SimpleEventSourcedRoot();
            origin.SetName("Jimi Hendrix");
            var updated = ValidationMessageAssert.IsValid(origin.SetName("Jimi Hendrix"));
            Assert.That(origin.Name, Is.Null);
            Assert.That(updated.Name, Is.EqualTo("Jimi Hendrix"));
        }

        [Test]
        public void without_matching_when_method_throws()
        {
            var aggregate = new TestApplyChangeAggregate();
            var exception = Assert.Catch<EventTypeNotSupportedException>(() => aggregate.TestApplyChange(new NameUpdated()));
            Assert.AreEqual(typeof(NameUpdated), exception.EventType);
        }
    }

    internal class TestApplyChangeAggregate : AggregateRoot<TestApplyChangeAggregate, Guid>
    {
        public TestApplyChangeAggregate()
            : base(Guid.NewGuid(), Qowaiv.Validation.Abstractions.Validator.Empty<TestApplyChangeAggregate>()) { }

        public void TestApplyChange(object @event)
        {
            ApplyEvent(@event);
        }
    }
}
