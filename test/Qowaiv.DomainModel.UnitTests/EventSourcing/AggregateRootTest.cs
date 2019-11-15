using NUnit.Framework;
using Qowaiv.DomainModel.EventSourcing;
using Qowaiv.DomainModel.UnitTests.Models;
using Qowaiv.Validation.Abstractions;
using System;

namespace Qowaiv.DomainModel.UnitTests.EventSourcing
{
    public class AggregateRootTest
    {
        [Test]
        public void FromEvents_AggregateShouldHaveIdOfEvents()
        {
            var aggregateId = Guid.Parse("4BC26714-F8B9-4E88-8435-BA8383B5DFC8");
            var stream = EventStream.FromMessages(new[] { new EventMessage(new EventInfo(1, aggregateId, Clock.UtcNow()), new SimpleInitEvent()) });

            var aggregate = AggregateRoot.FromEvents<SimpleEventSourcedRoot>(stream);

            Assert.AreEqual(aggregateId, aggregate.Id);
            Assert.AreEqual(aggregateId, aggregate.EventStream.AggregateId);
            Assert.AreEqual(1, aggregate.Version);
            Assert.AreEqual(1, aggregate.EventStream.CommittedVersion);
        }

        [Test]
        public void FromEvents_WithInvalidState_ShouldBeLoaded()
        {
            var aggregateId = Guid.Parse("4BC26714-F8B9-4E88-8435-BA8383B5DFC8");
            var stream = EventStream.FromMessages(new[] 
            { 
                new EventMessage(new EventInfo(1, aggregateId, Clock.UtcNow()), new SimpleInitEvent()),
                new EventMessage(new EventInfo(2, aggregateId, Clock.UtcNow()), new InvalidEvent())
            });

            var aggregate = AggregateRoot.FromEvents<SimpleEventSourcedRoot>(stream);

            Assert.IsTrue(aggregate.IsWrong);
            Assert.AreEqual(aggregateId, aggregate.Id);
            Assert.AreEqual(aggregateId, aggregate.EventStream.AggregateId);
            Assert.AreEqual(2, aggregate.Version);
            Assert.AreEqual(2, aggregate.EventStream.CommittedVersion);
        }

        [Test]
        public void SetProperty_ProofsThatValidatorWorks()
        {
            var aggregate = new SimpleEventSourcedRoot();
            Assert.Throws<InvalidModelException>(() => aggregate.IsWrong = true);
        }
    }
}
