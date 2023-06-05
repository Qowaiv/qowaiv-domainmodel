namespace Qowaiv.DomainModel.UnitTests.Models;

public sealed class SimpleEventSourcedAggregate : Aggregate<SimpleEventSourcedAggregate, Guid>
{
    public SimpleEventSourcedAggregate() : base(Guid.NewGuid(), new SimpleEventSourcedAggregateValidator()) { }

    public bool Initialized { get; private set; }

    public string? Name { get; private set; }

    public Date DateOfBirth { get; private set; }

    public bool IsWrong { get; private set; }

    public Result<SimpleEventSourcedAggregate> SetName(string name) => ApplyEvent(new NameUpdated { Name = name });

    public Result<SimpleEventSourcedAggregate> SetPerson(string name, Date dateOfBirth)
    {
        return ApplyEvents(
            new NameUpdated { Name = name },
            new DateOfBirthUpdated { DateOfBirth = dateOfBirth });
    }

    internal void When(NameUpdated @event)
    {
        Name = @event.Name;
    }

    internal void When(DateOfBirthUpdated @event)
    {
        DateOfBirth = @event.DateOfBirth;
    }

    internal void When(SimpleInitEvent @event)
    {
        Initialized = @event != null;
    }

    internal void When(InvalidEvent @event)
    {
        IsWrong = @event != null;
    }
}
