using NUnit.Framework;
using Qowaiv.DomainModel.EventSourcing;
using Qowaiv.TestTools;
using System;

namespace Qowaiv.DomainModel.UnitTests.EventSourcing
{
    public class EventMessageTest
    {
        [Test]
        public void DebuggerDisplay()
        {
            var info = new EventInfo(1, Guid.Parse("6E1D3455-D1E2-484E-8C54-27F9B0BFE8BA"));
            var @event = new ExampleEvent { Number = 17, Label = "Hello" };
            var message = new EventMessage(info, @event);

            DebuggerDisplayAssert.HasResult("Example, Version: 1, Props[2] { Number: 17, Label: Hello }, Aggregate: {6e1d3455-d1e2-484e-8c54-27f9b0bfe8ba}", message);
        }
    }

    internal class ExampleEvent
    {
        public int Number { get; set; }
        public string Label { get; set; }
    }

}
