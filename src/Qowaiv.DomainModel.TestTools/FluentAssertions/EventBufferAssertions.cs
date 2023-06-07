using FluentAssertions;
using FluentAssertions.Collections;
using Qowaiv.DomainModel.TestTools.Diagnostics.Contracts;
using System.Text;

namespace Qowaiv.DomainModel.TestTools.FluentAssertions;

/// <summary>Assertions on <see cref="EventBuffer{TId}"/>.</summary>
/// <typeparam name="TId">
/// The type of the identifier of the event buffer.
/// </typeparam>
[Inheritable]
[CLSCompliant(false)]
public class EventBufferAssertions<TId> : GenericCollectionAssertions<EventBuffer<TId>, object, EventBufferAssertions<TId>>
{
    /// <summary>Initializes a new instance of the <see cref="EventBufferAssertions{TId}"/> class.</summary>
    public EventBufferAssertions(EventBuffer<TId> subject) : base(subject) { }

    /// <summary>Verifies that the <see cref="AggregateRoot{TAggregate}"/> has the expected uncommitted events.</summary>
    /// <param name="uncommitted">
    /// The expected uncommitted event messages.
    /// </param>
    [Assertion]
    public AndConstraint<EventBufferAssertions<TId>> HaveUncommittedEvents(params object[] uncommitted)
    {
        Guard.HasAny(uncommitted, nameof(uncommitted));

        var sb = new StringBuilder();
        var failure = false;
        var offset = Subject.CommittedVersion;
        var actualEvents = Subject.Uncommitted.ToArray();
        var shared = Math.Min(uncommitted.Length, actualEvents.Length);

        if (actualEvents.Length == 0)
        {
            throw new AssertionFailed("There where no uncommitted events.");
        }

        for (var i = 0; i < shared; i++)
        {
            failure |= sb.AppendEvents(offset + i, uncommitted[i], actualEvents[i]);
        }

        failure |= sb.AppendExtraEvents(actualEvents, offset, shared, "Extra:  ");
        failure |= sb.AppendExtraEvents(uncommitted, offset, shared, "Missing: ");

        if (failure)
        {
            throw new AssertionFailed(sb.Insert(0, "The uncommitted events where different than expected." + Environment.NewLine).ToString());
        }

        return new(this);
    }

    /// <inheritdoc />
    protected override string Identifier => "event-buffer";
}
