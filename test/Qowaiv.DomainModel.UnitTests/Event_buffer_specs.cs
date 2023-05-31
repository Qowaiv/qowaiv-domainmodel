using Qowaiv.DomainModel;

namespace Event_buffer_specs;

public class Creation
{
    [Test]
    public void Empty_has_version_0()
        => EventBuffer.Empty(Guid.NewGuid()).Version.Should().Be(0);

    [TestCase(1)]
    [TestCase(17)]
    [TestCase(666)]
    public void Empty_with_initial_has_equals(int version)
        => EventBuffer.Empty(Guid.NewGuid(), version).Version.Should().Be(version);

    [TestCase]
    public void FromStorage_contains_all_events_as_committed()
    {
        var stored = new[] { new EmptyEvent(), new EmptyEvent(), new EmptyEvent() };
        var buffer = EventBuffer.FromStorage(Guid.NewGuid(), stored, (@event) => @event);

        buffer.Should().HaveCount(3)
            .And.Subject.As<EventBuffer<Guid>>().CommittedVersion.Should().Be(3);
    }

    [TestCase]
    public void FromStorage_takes_initial_version_into_account()
    {
        var stored = new[] { new EmptyEvent(), new EmptyEvent(), new EmptyEvent() };
        var buffer = EventBuffer.FromStorage(Guid.NewGuid(), initialVersion: 6, stored, (@event) => @event);

        buffer.Should().HaveCount(3)
         .And.Subject.As<EventBuffer<Guid>>().CommittedVersion.Should().Be(9);
    }
}

public class Select_uncommitted
{
    [Test]
    public void convert_uses_version_according_to_buffer()
    {
        var buffer = EventBuffer.Empty(Guid.NewGuid())
            .Add(new EmptyEvent())
            .Add(new EmptyEvent())
            .Add(new EmptyEvent())
            .MarkAllAsCommitted()
            .Add(new EmptyEvent())
            .Add(new EmptyEvent());

        var selected = buffer.SelectUncommitted((id, version, @event) => version);

        Assert.That(selected.Count(), Is.EqualTo(2));
        Assert.That(selected, Is.EqualTo(new[] { 4, 5 }));
    }

    [Test]
    public void convert_uses_aggregate_id()
    {
        var buffer = EventBuffer.Empty("my-id-007")
            .Add(new EmptyEvent())
            .Add(new EmptyEvent())
            .Add(new EmptyEvent());

        var selected = buffer.SelectUncommitted((id, version, @event) => new StoredEvent(id, version, @event));
        selected.Should().BeEquivalentTo(new[]
        {
            new StoredEvent("my-id-007", 1, new EmptyEvent()),
            new StoredEvent("my-id-007", 2, new EmptyEvent()),
            new StoredEvent("my-id-007", 3, new EmptyEvent()),
        });
    }
}

public class IsEmpty
{
    [Test]
    public void True_for_empty_buffer() => EventBuffer.Empty(17).IsEmpty.Should().BeTrue();

    [Test]
    public void False_for_non_empty_buffer() => EventBuffer.Empty(17).Add(new EmptyEvent()).IsEmpty.Should().BeFalse();
}

public class HasUncommitted
{
    [Test]
    public void False_for_empty_buffer() => EventBuffer.Empty(17).HasUncommitted.Should().BeFalse();

    [Test]
    public void True_for_non_empty_buffer() => EventBuffer.Empty(17).Add(new EmptyEvent()).HasUncommitted.Should().BeTrue();

    [Test]
    public void False_for_non_empty_buffer_all_marked_as_committed()
        => EventBuffer.Empty(17).Add(new EmptyEvent()).MarkAllAsCommitted().HasUncommitted.Should().BeFalse();
}

public class Debugger_display
{
    [Test]
    public void with_uncommitted()
    {
        object buffer = EventBuffer.Empty(Guid.Parse("1F8B5071-C03B-457D-B27F-442C5AAC5785"))
            .Add(new EmptyEvent());

        buffer.Should().HaveDebuggerDisplay("Version: 1 (Committed: 0), Aggregate: 1f8b5071-c03b-457d-b27f-442c5aac5785");
    }

    [Test]
    public void committed_only()
    {
        object buffer = EventBuffer.Empty(Guid.Parse("1F8B5071-C03B-457D-B27F-442C5AAC5785"))
            .Add(new EmptyEvent())
            .MarkAllAsCommitted();

        buffer.Should().HaveDebuggerDisplay("Version: 1, Aggregate: 1f8b5071-c03b-457d-b27f-442c5aac5785");
    }
}

[Obsolete("Will be dropped.")]
public class Obsolete_is
{
    [Test]
    public void Ctor_with_id()
        => Assert.That(() => new EventBuffer<int>(17), Is.Not.Null);

    [Test]
    public void Ctor_with_id_version()
        => Assert.That(() => new EventBuffer<int>(17, version: 12), Is.Not.Null);

    [Test]
    public void ClearCommitted()
        => Assert.That(() => EventBuffer.Empty(666).ClearCommitted(), Is.Not.Null);

    [Test]
    public void AddRange()
       => Assert.That(() => EventBuffer.Empty(666).AddRange(Array.Empty<object>()), Is.Not.Null);

    [Test]
    public void FromStorage()
        => Assert.That(() => EventBuffer<int>.FromStorage(1, Array.Empty<object>(), e => e), Is.Not.Null);

    [Test]
    public void FromStorage_with_version()
        => Assert.That(() => EventBuffer<int>.FromStorage(1, 0, Array.Empty<object>(), e => e), Is.Not.Null);
}

internal record EmptyEvent();
internal record StoredEvent(object Id, int Version, object Payload);

