using Qowaiv;
using Qowaiv.DomainModel;
using Qowaiv.DomainModel.UnitTests.Models;

namespace Event_sourced_aggregate_specs
{
    public class Replays_events
    {
        [Test]
        public void from_buffer()
        {
            var id = Guid.Parse("58B82A50-B906-4178-87EC-A8C31B49368B");
            var buffer = EventBuffer.Empty(id).Add(new NameUpdated { Name = "Jimi Hendrix" });

            var replayed = Aggregate.FromStorage<SimpleEventSourcedAggregate, Guid>(buffer);
            replayed.Should().BeEquivalentTo(new
            {
                Version = 1,
                Id = id,
            });
            replayed.Buffer.Should().HaveCount(1);
        }
    }

    public class Applies_events
    {
        [Test]
        public void changes_for_single_event()
        {
            var origin = new SimpleEventSourcedAggregate();
            var updated = origin.SetName("Jimi Hendrix").Should().BeValid().Value;

            updated.Name.Should().Be("Jimi Hendrix");
            updated.Buffer.Uncommitted.Should().BeEquivalentTo(new[] { new NameUpdated { Name = "Jimi Hendrix" } });
        }

        [Test]
        public void changes_for_multiple_events()
        {
            var origin = new SimpleEventSourcedAggregate();
            var updated = origin.SetPerson("Jimi Hendrix", new Date(1942, 11, 27)).Should().BeValid().Value;

            updated.Buffer.Uncommitted.Should().BeEquivalentTo(new object[]
            {
                new NameUpdated { Name = "Jimi Hendrix" },
                new DateOfBirthUpdated { DateOfBirth = new Date(1942, 11, 27) },
            });
        }

        [Test]
        public void as_uncommitted_to_the_buffer()
        {
            var origin = new SimpleEventSourcedAggregate();
            var updated =origin.SetName("Jimi Hendrix").Should().BeValid().Value;
            updated.Buffer.Uncommitted.Should().ContainSingle();
        }

        [Test]
        public void with_origin_unchanged()
        {
            var origin = new SimpleEventSourcedAggregate();
            origin.SetName("Jimi Hendrix");
            var updated = origin.SetName("Jimi Hendrix").Should().BeValid().Value;
            origin.Name.Should().BeNull();
            updated.Name.Should().Be("Jimi Hendrix");
        }

        [Test]
        public void skips_unknown_event_types()
        {
            var aggregate = new TestApplyChangeAggregate();
            var updated = aggregate.TestApplyChange(new NameUpdated()).Should().BeValid().Value;
            updated.Version.Should().Be(1);
        }
    }

    internal class TestApplyChangeAggregate : Aggregate<TestApplyChangeAggregate, Guid>
    {
        public TestApplyChangeAggregate()
            : base(Guid.NewGuid(), Qowaiv.Validation.Abstractions.Validator.Empty<TestApplyChangeAggregate>()) { }

        public Result<TestApplyChangeAggregate> TestApplyChange(object @event) => ApplyEvent(@event);
    }
}
