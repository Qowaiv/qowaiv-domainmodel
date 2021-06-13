using NUnit.Framework;
using Qowaiv.DomainModel.Events;
using System;

namespace EventCollection_specs
{
    internal static class Help
    {
        public static readonly bool? NotTrue;
        public static Dummy FailingCreation() => throw new DivideByZeroException();
    }
    internal class Dummy { }
    internal class Other { }

    public class Empty_collection
    {
        [Test]
        public void Has_size_of_0()
        {
            Assert.That(EventCollection.Empty, Has.Count.EqualTo(0));
        }

        [Test]
        public void Contains_no_items()
        {
            Assert.That(EventCollection.Empty, Is.EquivalentTo(Array.Empty<object>()));
        }

        [Test]
        public void Add_event_increases_the_size()
        {
            Assert.That(EventCollection.Empty.Add(new Dummy()), Has.Count.EqualTo(1));
        }
    }

    public class Non_empty_collection
    {
        [Test]
        public void Add_event_increases_the_size()
        {
            var events = EventCollection.Empty
                .Add(new Dummy())
                .Add(new Other());
            Assert.That(events, Has.Count.EqualTo(2));
        }
    }

    public class Add
    {
        [Test]
        public void Event_increases_the_size()
        {
            Assert.That(EventCollection.Empty.Add(new Dummy()), Has.Count.EqualTo(1));
        }
        
        [Test]
        public void Null_event_has_no_effect()
        {
            Assert.That(EventCollection.Empty.Add<object>(null), Has.Count.EqualTo(0));
        }

        [Test]
        public void Only_null_events_has_no_effect()
        {
            Assert.That(EventCollection.Empty.Add(null, null, null), Has.Count.EqualTo(0));
        }

        [Test]
        public void String_event_is_not_allowed()
        {
            Assert.Catch<ArgumentException>(() => EventCollection.Empty.Add("not allowed"));
        }
    }

    public class If_not_true
    {
        [Test]
        public void Then_not_excecuted()
        {
            var events = EventCollection.Empty
                .If(Help.NotTrue)
                .Then(Help.FailingCreation);

            Assert.That(events, Has.Count.EqualTo(0));
        }

        [Test]
        public void Else_increases_size()
        {
            var events = EventCollection.Empty
                .If(Help.NotTrue)
                .Then(Help.FailingCreation)
                .Else(()=> new Dummy());

            Assert.That(events, Has.Count.EqualTo(1));
        }
    }

    public class If_false
    {
        [Test]
        public void Then_not_excecuted()
        {
            var events = EventCollection.Empty
                .If(false)
                .Then(Help.FailingCreation);

            Assert.That(events, Has.Count.EqualTo(0));
        }

        [Test]
        public void Then_increases_size()
        {
            var events = EventCollection.Empty
                .If(false)
                .Then(Help.FailingCreation)
                .Else(() => new Dummy());

            Assert.That(events, Has.Count.EqualTo(1));
        }
    }

    public class If_true
    {
        [Test]
        public void Then_increases_size()
        {
            var events = EventCollection.Empty
                .If(true)
                .Then(() => new Dummy());

            Assert.That(events, Has.Count.EqualTo(1));
        }

        [Test]
        public void Else_not_executed()
        {
            var events = EventCollection.Empty
                .If(true)
                .Then(() => new Dummy())
                .Else(Help.FailingCreation);

            Assert.That(events, Has.Count.EqualTo(1));
        }
    }
}
