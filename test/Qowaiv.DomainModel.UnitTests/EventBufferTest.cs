using NUnit.Framework;
using Qowaiv.TestTools;
using System;
using System.Collections;
using System.Linq;

namespace Qowaiv.DomainModel.UnitTests
{
    public class EventBufferTest
    {
        private static readonly ConvertFromStoredEvent<object> fromStorage = (@event) => @event;
        private static readonly ConvertToStoredEvent<Guid, StoredEvent> select = (id, version, @event) => new StoredEvent { Id = id, Version = version, Payload = @event };

        [Test]
        public void FromStorage_CreatedSuccesfully()
        {
            var id = Guid.Parse("7231B710-77CD-11E9-8F9E-2A86E4085A59");

            var messages = new object[] { new DummyEvent(), new DummyEvent() };

            var buffer = EventBuffer.FromStorage(id, 17, messages, fromStorage);

            Assert.IsNotNull(buffer);
            Assert.AreEqual(id, buffer.AggregateId);
            Assert.AreEqual(19, buffer.Version);
        }

        [Test]
        public void SelectUncommitted_SelectedSuccessfully()
        {
            var id = Guid.Parse("7231B710-77CD-11E9-8F9E-2A86E4085A59");

            var buffer = EventBuffer.Empty(id, 17)
                .Add(new DummyEvent())
                .MarkAllAsCommitted()
                .Add(new DummyEvent());

            var selected = buffer.SelectUncommitted(select).ToArray();

            Assert.AreEqual(1, selected.Length);
            Assert.AreEqual(id, selected[0].Id);
            Assert.AreEqual(19, selected[0].Version);
            Assert.IsInstanceOf<DummyEvent>(selected[0].Payload);
        }

        [Test]
        public void HasUncommitted_OnlyCommitted_IsFalse()
        {
            var buffer = EventBuffer.Empty(Guid.NewGuid())
                .Add(new DummyEvent())
                .Add(new DummyEvent())
                .MarkAllAsCommitted();

            Assert.IsFalse(buffer.HasUncommitted);
        }

        [Test]
        public void HasUncommitted_Mixed_IsTrue()
        {
            var buffer = EventBuffer.Empty(Guid.NewGuid())
                .Add(new DummyEvent())
                .Add(new DummyEvent())
                .MarkAllAsCommitted()
                .Add(new DummyEvent());

            Assert.IsTrue(buffer.HasUncommitted);
        }

        [Test]
        public void IsEmpty_Empty_IsTrue()
        {
            var buffer = EventBuffer.Empty(Guid.NewGuid());
            
            Assert.IsTrue(buffer.IsEmpty);
        }

        [Test]
        public void IsEmpty_Mixed_IsFalse()
        {
            var buffer = EventBuffer.Empty(Guid.NewGuid())
                .Add(new DummyEvent());

            Assert.IsFalse(buffer.IsEmpty);
        }

        [Test]
        public void GetUncommitted_NoCommitted_All()
        {
            var buffer = EventBuffer.Empty(Guid.NewGuid())
                .Add(new object[]
                {
                    new DummyEvent(),
                    new DummyEvent(),
                    new DummyEvent()
                });
            Assert.AreEqual(3, buffer.Uncommitted.Count());
        }

        [Test]
        public void GetUncommitted_3Committed_1ItemWithVersion4()
        {
            var buffer = EventBuffer.Empty(Guid.NewGuid())
                .Add(new object[]
                {
                    new DummyEvent(),
                    new DummyEvent(),
                    new DummyEvent()
                })
                .MarkAllAsCommitted()
                .ClearCommitted()
                .Add(new DummyEvent());

            var uncommited = buffer.Uncommitted.ToArray();

            Assert.AreEqual(1, uncommited.Length);
            Assert.AreEqual(1, buffer.Count());
            Assert.AreEqual(4, buffer.Version);
        }

        [Test]
        public void GetUncommitted_ClearCommittedWithCommitted_1ItemWithVersion5()
        {
            var buffer = EventBuffer.Empty(Guid.NewGuid())
                .Add(new[]
                {
                    new DummyEvent(),
                    new DummyEvent(),
                    new DummyEvent()
                })
                .MarkAllAsCommitted()
                .ClearCommitted()
                .Add(new DummyEvent())
                .MarkAllAsCommitted()
                .Add(new DummyEvent());

            var uncommited = buffer.Uncommitted.ToArray();

            Assert.AreEqual(1, uncommited.Length);
            Assert.AreEqual(2, buffer.Count());
            Assert.AreEqual(5, buffer.Version);
        }

        [Test]
        public void ClearCommitted_WithoutUncommitted()
        {
            var buffer = EventBuffer.Empty(Guid.NewGuid(), 16);

            Assert.AreEqual(16, buffer.Version);
            Assert.AreEqual(16, buffer.CommittedVersion);
            Assert.IsEmpty(buffer.Uncommitted);
        }

        [Test]
        public void ClearCommitted_WithUncommitted()
        {
            var buffer = EventBuffer.Empty(Guid.NewGuid(), 16).Add(new DummyEvent());
            Assert.AreEqual(17, buffer.Version);
            Assert.AreEqual(16, buffer.CommittedVersion);
            Assert.AreEqual(1, buffer.Uncommitted.Count());
            Assert.AreEqual(1, buffer.Count());
        }

        [Test]
        public void DebuggerDisplay_WithUncommited()
        {
            using (Clock.SetTimeForCurrentThread(() => new DateTime(2017, 06, 11)))
            {
                var buffer = EventBuffer.Empty(Guid.Parse("1F8B5071-C03B-457D-B27F-442C5AAC5785"))
                    .Add(new DummyEvent());

                DebuggerDisplayAssert.HasResult("Version: 1 (Committed: 0), Aggregate: 1f8b5071-c03b-457d-b27f-442c5aac5785", buffer);
            }
        }

        [Test]
        public void DebuggerDisplay_WithoutUncommited()
        {
            using (Clock.SetTimeForCurrentThread(() => new DateTime(2017, 06, 11)))
            {
                var buffer = EventBuffer.Empty(Guid.Parse("1F8B5071-C03B-457D-B27F-442C5AAC5785"))
                    .Add(new DummyEvent())
                    .MarkAllAsCommitted();

                DebuggerDisplayAssert.HasResult("Version: 1, Aggregate: 1f8b5071-c03b-457d-b27f-442c5aac5785", buffer);
            }
        }

        [Test]
        public void GetEnumerator_NonGeneric_ShouldReturn()
        {
            var buffer = EventBuffer.Empty(Guid.NewGuid())
               .Add(new DummyEvent())
               .Add(new DummyEvent());

            var enumerable = (IEnumerable)buffer;

            Assert.AreEqual(2, enumerable.Cast<DummyEvent>().Count());
        }

        private class DummyEvent { }

        private class StoredEvent 
        {
            public Guid Id { get; set; }
            public int Version { get; set; }
            public object Payload { get; set; }
        }
    }
}
