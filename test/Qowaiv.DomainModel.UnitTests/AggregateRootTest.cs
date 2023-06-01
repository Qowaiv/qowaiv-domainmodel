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

        aggregate.Should().BeEquivalentTo(new
        {
            Id = aggregateId,
            Version = 1,
            Buffer = new
            {
                AggregateId = aggregateId,
                CommittedVersion = 1,
            }
        });
    }

    [Test]
    public void FromEvents_WithInvalidState_ShouldBeLoaded()
    {
        var aggregateId = Guid.Parse("4BC26714-F8B9-4E88-8435-BA8383B5DFC8");
        var buffer = Create(aggregateId, new SimpleInitEvent(), new InvalidEvent());
        var aggregate = AggregateRoot.FromStorage<SimpleEventSourcedRoot, Guid>(buffer);

        aggregate.Should().BeEquivalentTo(new
        {
            IsWrong = true,
            Id = aggregateId,
            Version = 2,
            Buffer = new
            {
                AggregateId = aggregateId,
                CommittedVersion = 2,
            }
        });
    }

    private static EventBuffer<Guid> Create(Guid id, params object[] events) 
        => EventBuffer.FromStorage(id, events, o => o);
}
