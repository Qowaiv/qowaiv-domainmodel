using NUnit.Framework;
using Qowaiv.DomainModel.Collections;
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
            Assert.That(ImmutableCollection.Empty, Has.Count.EqualTo(0));
        }

        [Test]
        public void Contains_no_items()
        {
            Assert.That(ImmutableCollection.Empty, Is.EquivalentTo(Array.Empty<object>()));
        }

        [Test]
        public void Add_event_increases_the_size()
        {
            Assert.That(ImmutableCollection.Empty.Add(new Dummy()), Has.Count.EqualTo(1));
        }
    }

    public class Non_empty_collection
    {
        [Test]
        public void Add_event_increases_the_size()
        {
            var events = ImmutableCollection.Empty
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
            Assert.That(ImmutableCollection.Empty.Add(new Dummy()), Has.Count.EqualTo(1));
        }
        
        [Test]
        public void Null_event_has_no_effect()
        {
            Assert.That(ImmutableCollection.Empty.Add<object>(null), Has.Count.EqualTo(0));
        }

        [Test]
        public void Only_null_events_has_no_effect()
        {
            Assert.That(ImmutableCollection.Empty.Add(null, null, null), Has.Count.EqualTo(0));
        }

        [Test]
        public void String_event_is_not_allowed()
        {
            Assert.Catch<ArgumentException>(() => ImmutableCollection.Empty.Add("not allowed"));
        }
    }

    public class If_not_true
    {
        [Test]
        public void Then_not_executed()
        {
            var events = ImmutableCollection.Empty
                .If(Help.NotTrue)
                .Then(Help.FailingCreation);

            Assert.That(events, Has.Count.EqualTo(0));
        }

        [Test]
        public void Else_increases_size()
        {
            var events = ImmutableCollection.Empty
                .If(Help.NotTrue)
                    .Then(Help.FailingCreation)
                .Else(()=> new Dummy());

            Assert.That(events, Has.Count.EqualTo(1));
        }
    }

    public class If_false
    {
        [Test]
        public void Then_not_executed()
        {
            var events = ImmutableCollection.Empty
                .If(false)
                .Then(Help.FailingCreation);

            Assert.That(events, Has.Count.EqualTo(0));
        }

        [Test]
        public void Then_increases_size()
        {
            var events = ImmutableCollection.Empty
                .If(false)
                    .Then(Help.FailingCreation)
                .Else(() => new Dummy());

            Assert.That(events, Has.Count.EqualTo(1));
        }

        [Test]
        public void Else_if_true_increases_size()
        {
            var events = ImmutableCollection.Empty
                .If(false)
                    .Then(Help.FailingCreation)
                .ElseIf(true)
                    .Then(() => new Dummy())
                .Else(Help.FailingCreation);

            Assert.That(events, Has.Count.EqualTo(1));
        }

        [Test]
        public void Else_if_false_not_executed()
        {
            var events = ImmutableCollection.Empty
                .If(false)
                    .Then(Help.FailingCreation)
                .ElseIf(false)
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
            var events = ImmutableCollection.Empty
                .If(true)
                .Then(() => new Dummy());

            Assert.That(events, Has.Count.EqualTo(1));
        }

        [Test]
        public void Else_not_executed()
        {
            var events = ImmutableCollection.Empty
                .If(true)
                    .Then(() => new Dummy())
                .Else(Help.FailingCreation);

            Assert.That(events, Has.Count.EqualTo(1));
        }

        [Test]
        public void Else_if_not_executed()
        {
            var events = ImmutableCollection.Empty
                .If(true)
                    .Then(() => new Dummy())
                .ElseIf(true)
                    .Then(Help.FailingCreation)
                .Else(Help.FailingCreation);

            Assert.That(events, Has.Count.EqualTo(1));
        }
    }
}
