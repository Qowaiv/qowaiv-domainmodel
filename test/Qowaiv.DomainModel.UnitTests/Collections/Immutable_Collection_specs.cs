using FluentAssertions;
using NUnit.Framework;
using Qowaiv.DomainModel.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Collections.Immutable_Collection_specs
{
    internal static class Help
    {
        public static readonly bool? NotTrue = default;
        public static Dummy FailingCreation() => throw new DivideByZeroException();
    }
    internal class Dummy { }
    internal class Other { }

    internal class Dummies : IEnumerable<Dummy>
    {
        public int Count { get; private set; }

        public IEnumerator<Dummy> GetEnumerator()
            => Enumerable.Repeat(new Dummy(), ++Count).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class Empty_collection
    {
        [Test]
        public void Has_size_of_0()
            => ImmutableCollection.Empty.Should().BeEmpty();

        [Test]
        public void Contains_no_items()
            => ImmutableCollection.Empty.Should().BeEquivalentTo(Array.Empty<object>());

        [Test]
        public void Add_item_increases_the_size()
            => ImmutableCollection.Empty.Add(new Dummy()).Should().ContainSingle();
    }

    public class Non_empty_collection
    {
        [Test]
        public void Add_item_increases_the_size()
            => ImmutableCollection.Empty
                .Add(new Dummy())
                .Add(new Other())
            .Should().HaveCount(2);
    }

    public class Add
    {
        [Test]
        public void Event_increases_the_size()
            => ImmutableCollection.Empty.Add(new Dummy()).Should().ContainSingle();

        [Test]
        public void Null_item_has_no_effect()
            => ImmutableCollection.Empty.Add<object>(null).Should().BeEmpty();

        [Test]
        public void Only_null_items_has_no_effect()
            => ImmutableCollection.Empty.Add(null, null, null).Should().BeEmpty();

        [Test]
        public void String_is_not_considered_an_collection()
            => ImmutableCollection.Empty.Add("some string").Should().ContainSingle();
    }

    public class If_not_true
    {
        [Test]
        public void Then_not_executed()
        {
            var events = ImmutableCollection.Empty
                .If(Help.NotTrue)
                .Then(Help.FailingCreation);

            events.Should().BeEmpty();
        }

        [Test]
        public void Else_increases_size()
        {
            var events = ImmutableCollection.Empty
                .If(Help.NotTrue)
                    .Then(Help.FailingCreation)
                .Else(() => new Dummy());

            events.Should().ContainSingle();
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

            events.Should().BeEmpty();
        }

        [Test]
        public void Then_increases_size()
        {
            var events = ImmutableCollection.Empty
                .If(false)
                    .Then(Help.FailingCreation)
                .Else(() => new Dummy());

            events.Should().ContainSingle();
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

            events.Should().ContainSingle();
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

            events.Should().ContainSingle();
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

            events.Should().ContainSingle();
        }

        [Test]
        public void Else_not_executed()
        {
            var events = ImmutableCollection.Empty
                .If(true)
                    .Then(() => new Dummy())
                .Else(Help.FailingCreation);

            events.Should().ContainSingle();
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

            events.Should().ContainSingle();
        }
    }

    public class Enumerables
    {
        [Test]
        public void are_added_as_fixed_list()
        {
            var events = ImmutableCollection.Empty.Add(new Dummies());
            events.Should().ContainSingle(because: "the initial count is 1.");
            events.Should().ContainSingle(because: "calling it multiple times should not change a thing.");
        }
    }
}
