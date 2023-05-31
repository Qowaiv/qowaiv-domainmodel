using Qowaiv.DomainModel;
using Qowaiv.DomainModel.UnitTests.Models;
using Qowaiv.TestTools;

namespace EventTypeNotSupported_specs;

public class Serialize_RoundTrip
{
    [Test]
    public void RoundTrip_keeps_not_supported_type()
    {
        var exception = new EventTypeNotSupported(typeof(int), typeof(SimpleEventSourcedRoot));
        var actual = SerializeDeserialize.Xml(exception);

        Assert.AreEqual(typeof(int), actual.EventType);
        Assert.AreEqual(typeof(SimpleEventSourcedRoot), actual.AggregateType);
    }
}
