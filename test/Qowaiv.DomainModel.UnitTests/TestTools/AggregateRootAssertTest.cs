using NUnit.Framework;
using Qowaiv.DomainModel.EventSourcing;
using Qowaiv.DomainModel.TestTools;
using Qowaiv.DomainModel.TestTools.EventSourcing;
using Qowaiv.Globalization;
using System;
using TestEvents;

namespace Qowaiv.DomainModel.UnitTests.TestTools
{
    public class AggregateRootAssertTest
    {
        [Test]
        public void HasUncommittedEvents_IsTrue()
        {
            var stream = new EventStream();
            stream.Add(new EmptyEvent());
            stream.Add(new SimpleEvent { Value = 3 });
            stream.Add(new OtherEvent { Value = 3 });

            AggregateRootAssert.HasUncommittedEvents(stream,
                new EmptyEvent(),
                new SimpleEvent { Value = 3 },
                new OtherEvent { Value = 3 }
            );
        }

        [Test]
        public void HasUncommittedEvents_EqualArrayValues_IsTrue()
        {
            var stream = new EventStream();
            stream.Add(new ArrayEvent { Numbers = new[] { 17 } });

            AggregateRootAssert.HasUncommittedEvents(stream,
               new ArrayEvent { Numbers = new[] { 17 } });
        }

        [Test]
        public void HasUncommittedEvents_DiffrentEventTypes_DisplayedInTheMessage()
        {
            var stream = new EventStream();
            stream.Add(new EmptyEvent());
            stream.Add(new SimpleEvent());
            stream.Add(new SimpleEvent());

            var x = Assert.Catch<AssertException>(() =>
                AggregateRootAssert.HasUncommittedEvents(stream,
                    new EmptyEvent(),
                    new OtherEvent(),
                    new SimpleEvent()
                ));

            Assert.AreEqual(@"Assertion failed:
[0] EmptyEvent
[1] Expected: TestEvents.OtherEvent
    Actual:   TestEvents.SimpleEvent
[2] SimpleEvent
", x.Message);
        }

        [Test]
        public void HasUncommittedEvents_DiffrentEventValues_DisplayedInTheMessage()
        {
            using (CultureInfoScope.NewInvariant())
            {
                var stream = new EventStream();
                stream.Add(new EmptyEvent());
                stream.Add(new Complex { Value = 17, Message = "Same", Date = new DateTime(2017, 06, 11) });
                stream.Add(new SimpleEvent());

                var x = Assert.Catch<AssertException>(() =>
                    AggregateRootAssert.HasUncommittedEvents(stream,
                        new EmptyEvent(),
                        new Complex { Value = 23, Message = "Same", Date = new DateTime(1980, 06, 30) },
                        new SimpleEvent()
                    ));

                Assert.AreEqual(@"Assertion failed:
[0] EmptyEvent
[1] Expected: { Value: 23, Date: 06/30/1980 00:00:00 }
    Actual:   { Value: 17, Date: 06/11/2017 00:00:00 }
[2] SimpleEvent
", x.Message);
            }
        }

        [Test]
        public void HasUncommittedEvents_WithExtraEvents_DisplayedInTheMessage()
        {
            var stream = new EventStream();
            stream.Add(new EmptyEvent());
            stream.MarkAllAsCommitted();
            stream.Add(new EmptyEvent());
            stream.Add(new SimpleEvent { Value = 3 });
            stream.Add(new OtherEvent { Value = 3 });

            var x = Assert.Catch<AssertException>(() =>
                AggregateRootAssert.HasUncommittedEvents(stream,
                    new EmptyEvent(),
                    new SimpleEvent { Value = 3 }
                ));

            Assert.AreEqual(@"Assertion failed:
[1] EmptyEvent
[2] SimpleEvent
[3] Extra:   OtherEvent
", x.Message);
        }

        [Test]
        public void HasUncommittedEvents_WithMissingEvents_DisplayedInTheMessage()
        {
            var stream = new EventStream();
            stream.Add(new EmptyEvent());
            stream.Add(new OtherEvent { Value = 3 });

            var x = Assert.Catch<AssertException>(() =>
                AggregateRootAssert.HasUncommittedEvents(stream,
                    new EmptyEvent(),
                    new OtherEvent { Value = 3 },
                    new SimpleEvent { Value = 5 },
                    new OtherEvent { Value = 9 }
                ));

            Assert.AreEqual(@"Assertion failed:
[0] EmptyEvent
[1] OtherEvent
[2] Missing:  SimpleEvent
[3] Missing:  OtherEvent
", x.Message);
        }


        [Test]
        public void HasUncommittedEvents_DifferentArrayValues_DisplayedInTheMessage()
        {
            var stream = new EventStream();
            stream.Add(new ArrayEvent { Numbers = new[] { 17 } });

            var x = Assert.Catch<AssertException>(() =>
               AggregateRootAssert.HasUncommittedEvents(stream,
                   new ArrayEvent { Numbers = new[] { 18 } }
               ));

            Assert.AreEqual(@"Assertion failed:
[0] Expected: { Numbers: { 18 } }
    Actual:   { Numbers: { 17 } }
", x.Message);
        }
    }
}

namespace TestEvents
{
    internal class EmptyEvent { }
    internal class SimpleEvent { public int Value { get; set; } }
    internal class OtherEvent { public int Value { get; set; } }
    internal class Complex
    {
        public int Value { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
    }
    internal class ArrayEvent { public int[] Numbers { get; set; } }
}
