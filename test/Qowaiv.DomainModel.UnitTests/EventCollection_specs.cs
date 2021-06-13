using NUnit.Framework;
using Qowaiv.DomainModel;
using System;
using System.Linq;

namespace EventCollection_specs
{
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
        public void When_not_true_keeps_size_of_0()
        {
            Assert.That(EventCollection.Empty.When(false), Has.Count.EqualTo(0));
        }
    }
    public class Add
    {
        [Test]
        public void Event_increases_the_size()
        {
            Assert.That(EventCollection.Empty.Add(new object()), Has.Count.EqualTo(1));
        }
        
        [Test]
        public void Null_returns_an_unchanged_state()
        {
            Assert.That(EventCollection.Empty.Add<object>(null), Has.Count.EqualTo(0));
        }

        [Test]
        public void All_null_returns_an_unchanged_state()
        {
            Assert.That(EventCollection.Empty.Add(null, null, null), Has.Count.EqualTo(0));
        }

        [Test]
        public void Lazy_event_increases_the_size()
        {
            Assert.That(EventCollection.Empty.Add(() => new object()), Has.Count.EqualTo(1));
        }
    }

    public class When_not_true
    {
        [Test]
        public void Last_addition_is_reversed()
        {
            var events = EventCollection.Empty
                .Add(new object()).When(default(bool?));

            Assert.That(events, Has.Count.EqualTo(0));
        }

        [Test]
        public void Lazy_addition_is_not_executed()
        {
            static object lazy() => throw new Exception("failure");
            var events = EventCollection.Empty
                .Add(lazy).When(default(bool?));

            Assert.That(events, Has.Count.EqualTo(0));
        }
    }

    public class When_false
    {
        [Test]
        public void Last_addition_is_reversed()
        {
            var events = EventCollection.Empty
                .Add(new object()).When(false);
            
            Assert.That(events, Has.Count.EqualTo(0));
        }

        [Test]
        public void Lazy_addition_is_not_executed()
        {
            static object lazy() => throw new Exception("failure");
            var events = EventCollection.Empty
                .Add(lazy).When(false);

            Assert.That(events, Has.Count.EqualTo(0));
        }
    }

    public class When_true
    {
        [Test]
        public void Last_addition_is_not_reversed()
        {
            var events = EventCollection.Empty
                .Add(new object()).When(true);

            Assert.That(events, Has.Count.EqualTo(1));
        }

        [Test]
        public void Lazy_addition_is_executed()
        {
            static object lazy() => throw new ApplicationException("failure");
            var events = EventCollection.Empty
                .Add(lazy).When(true);

            Assert.Throws<ApplicationException>(() => events.ToArray());
        }
    }
}
