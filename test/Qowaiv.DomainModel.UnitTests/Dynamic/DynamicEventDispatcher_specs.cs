using Microsoft.CSharp.RuntimeBinder;
using NUnit.Framework;
using Qowaiv.DomainModel.Dynamic;
using System;

namespace Dynamic_event_dispatcher_specs
{
    public class Support_when_methods
    {
        [Test]
        public void With_single_argument() => AssertEventTypes<WhenWithReturnStatement>();
        [Test]
        public void With_return_statement() => AssertEventTypes<WhenWithReturnStatement>();
        [Test]
        public void Non_public_methods()
        {
            dynamic dispatcher = new DynamicEventDispatcher<WhenWithSingleArgument>(new WhenWithSingleArgument());
            Assert.Catch<Detected>(() => dispatcher.When(new DummyEvent()));
        }

        internal class WhenWithSingleArgument
        {
            internal void When(DummyEvent @event) => throw new Detected();
        }

        internal class WhenWithReturnStatement
        {
            internal int When(DummyEvent @event) => throw new Detected();
        }

        private static void AssertEventTypes<T>() where T : class, new()
        {
            var dynamic = new DynamicEventDispatcher<T>(new T());
            Assert.AreEqual(1, dynamic.SupportedEventTypes.Count);
        }

        internal class DummyEvent { }

#pragma warning disable S4027 // Exceptions should provide standard constructors
        public class Detected : Exception { }
#pragma warning restore S4027 // Exceptions should provide standard constructors
    }

    public class Not_supported_when_methods
    {
        [Test]
        public void Object_argument() => AssertNoEventTypes<WhenWithObjectParameter>();
        
        [Test]
        public void Primitive_arguments() => AssertNoEventTypes<WhenWithPrimitiveParameters>();

        [Test]
        public void Enum_arguments() => AssertNoEventTypes<WhenWithEnumParameters>();

        [Test]
        public void No_arguments() => AssertNoEventTypes<WhenNoParameterEvent>();

        [Test]
        public void Multiple_arguments() => AssertNoEventTypes<WhenWithMultipleParameters>();

        private static void AssertNoEventTypes<T>() where T: class, new()
        {
            var dynamic = new DynamicEventDispatcher<T>(new T());
            Assert.IsEmpty(dynamic.SupportedEventTypes);
        }

        [Test]
        public void Fallback_to_base()
        {
            dynamic dynamic = new DynamicEventDispatcher<WhenWithMultipleParameters>(new WhenWithMultipleParameters());
            Assert.IsEmpty(dynamic.SupportedEventTypes);
            Assert.Throws<RuntimeBinderException>(() => dynamic.When(new DummyEvent { }, 4));
        }

        internal class WhenWithMultipleParameters
        {
            internal void When(DummyEvent @event, int other) => throw new NotSupportedException($"{@event} + {other}.");
        }

        internal class WhenNoParameterEvent
        {
            internal void When() => throw new NotSupportedException();
        }

        internal class WhenWithObjectParameter
        {
            internal void When(object @event) => throw new NotSupportedException($"Not a valid event: {@event}.");
        }
        internal class WhenWithPrimitiveParameters
        {
            internal void When(int @event) => throw new NotSupportedException($"Not a valid event: {@event}.");
            internal void When(long @event) => throw new NotSupportedException($"Not a valid event: {@event}.");
            internal void When(bool @event) => throw new NotSupportedException($"Not a valid event: {@event}.");
        }
        internal class WhenWithEnumParameters
        {
            internal void When(Base64FormattingOptions @event) => throw new NotSupportedException($"Not a valid event: {@event}.");
            internal void When(EnvironmentVariableTarget @event) => throw new NotSupportedException($"Not a valid event: {@event}.");
        }
        internal class DummyEvent { }
    }
}
