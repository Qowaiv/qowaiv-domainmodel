using Microsoft.CSharp.RuntimeBinder;
using NUnit.Framework;
using Qowaiv.DomainModel.Dynamic;
using System;

namespace Qowaiv.DomainModel.UnitTests.Dynamic
{
    public class DynamicEventDispatcherTest
    {
        [Test]
        public void Init_OnlyNotSupportedEventTypes_SupportedEventTypesIsEmpty()
        {
            var dynamic = new DynamicEventDispatcher<OnlyNotSupportedEventTypes>(new OnlyNotSupportedEventTypes());
            Assert.IsEmpty(dynamic.SupportedEventTypes);
        }

        [Test]
        public void TryInvokeMember_ViaBase_ThrowsRuntimeBinderException()
        {
            dynamic dynamic = new DynamicEventDispatcher<NotMatchingSignatures>(new NotMatchingSignatures());

            Assert.Throws<RuntimeBinderException>(() => dynamic.When(new DummyEvent { }, 4));
        }

        internal class NotMatchingSignatures
        {
            internal void When(DummyEvent @event, int other) => throw new NotSupportedException($"{@event} + {other}.");
        }

        internal class OnlyNotSupportedEventTypes
        {
            internal void When(object @event) => throw new NotSupportedException($"Not a valid event: {@event}.");
            internal void When(int @event) => throw new NotSupportedException($"Not a valid event: {@event}.");
            internal void When(Base64FormattingOptions @event) => throw new NotSupportedException($"Not a valid event: {@event}.");
        }
        internal class DummyEvent { }
    }
}
