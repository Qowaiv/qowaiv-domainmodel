namespace Benchmarks.Collections;

public sealed class Aggregate_id : AggregateRoot<Aggregate_id, int>
{
    public Aggregate_id()
        : base(default, Qowaiv.Validation.Abstractions.Validator.Empty<Aggregate_id>()) { }

    public double Sum { get; private set; }

    public Aggregate_id Add(object e) => ApplyEvent(e).Value;

    internal void When(Added e) => Sum += e.Addition;
}
