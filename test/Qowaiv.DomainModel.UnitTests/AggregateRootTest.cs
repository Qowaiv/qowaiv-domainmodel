using NUnit.Framework;
using Qowaiv.DomainModel.UnitTests.Models;
using System;

namespace Qowaiv.DomainModel.UnitTests
{
    public class AggregateRootTest
    {
        [Test]
        public void FromEvents_AggregateShouldHaveIdOfEvents()
        {
            var aggregateId = Guid.Parse("4BC26714-F8B9-4E88-8435-BA8383B5DFC8");
            var stream = new EventStream(aggregateId);
            stream.Add(new SimpleInitEvent());
            stream.MarkAllAsCommitted(clearCommitted: false);

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
            var stream = new EventStream(aggregateId);
            stream.AddRange(new SimpleInitEvent(), new InvalidEvent());
            stream.MarkAllAsCommitted(clearCommitted: false);

            var aggregate = AggregateRoot.FromEvents<SimpleEventSourcedRoot>(stream);

            Assert.IsTrue(aggregate.IsWrong);
            Assert.AreEqual(aggregateId, aggregate.Id);
            Assert.AreEqual(aggregateId, aggregate.EventStream.AggregateId);
            Assert.AreEqual(2, aggregate.Version);
            Assert.AreEqual(2, aggregate.EventStream.CommittedVersion);
        }
    }
}
