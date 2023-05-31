using Qowaiv.DomainModel.UnitTests.Models;

namespace Qowaiv.DomainModel.UnitTests;

public class AggregateRootTest
{
    [Test]
    public void FromEvents_AggregateShouldHaveIdOfEvents()
    {
        var aggregateId = Guid.Parse("4BC26714-F8B9-4E88-8435-BA8383B5DFC8");
        var buffer = Create(aggregateId, new SimpleInitEvent());
        var aggregate = AggregateRoot.FromStorage<SimpleEventSourcedRoot, Guid>(buffer);

        Assert.AreEqual(aggregateId, aggregate.Id);
        Assert.AreEqual(aggregateId, aggregate.Buffer.AggregateId);
        Assert.AreEqual(1, aggregate.Version);
        Assert.AreEqual(1, aggregate.Buffer.CommittedVersion);
    }

    [Test]
    public void FromEvents_WithInvalidState_ShouldBeLoaded()
    {
        var aggregateId = Guid.Parse("4BC26714-F8B9-4E88-8435-BA8383B5DFC8");
        var buffer = Create(aggregateId, new SimpleInitEvent(), new InvalidEvent());
        var aggregate = AggregateRoot.FromStorage<SimpleEventSourcedRoot, Guid>(buffer);

        Assert.IsTrue(aggregate.IsWrong);
        Assert.AreEqual(aggregateId, aggregate.Id);
        Assert.AreEqual(aggregateId, aggregate.Buffer.AggregateId);
        Assert.AreEqual(2, aggregate.Version);
        Assert.AreEqual(2, aggregate.Buffer.CommittedVersion);
    }


    private static EventBuffer<Guid> Create(Guid id, params object[] events)
    {
        return EventBuffer.FromStorage(id, events, o => o);
    }
}
