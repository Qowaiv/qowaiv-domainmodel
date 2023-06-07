using System.Diagnostics.Contracts;

namespace Qowaiv.DomainModel.UnitTests.Models;

internal sealed class AggregateStub : Aggregate<AggregateStub, int>
{
    public AggregateStub()
        : base(0, Validation.Abstractions.Validator.Empty<AggregateStub>()) { }

    [Pure]
    public AggregateStub Add(params object[] events)
        => ApplyEvents(events).Value;
}
