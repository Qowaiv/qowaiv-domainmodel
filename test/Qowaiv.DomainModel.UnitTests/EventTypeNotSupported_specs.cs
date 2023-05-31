using Qowaiv.DomainModel;
using Qowaiv.DomainModel.UnitTests.Models;
using Qowaiv.TestTools;

namespace EventTypeNotSupported_specs;

public class Serialize_RoundTrip
{
    [Test]
    [Obsolete("Usage of the binary formatter is considered harmful.")]
    public void RoundTrip_keeps_not_supported_type()
    {
        var exception = new EventTypeNotSupported(typeof(int), typeof(SimpleEventSourcedRoot));
        SerializeDeserialize.Binary(exception)
            .Should().BeEquivalentTo(new
            {
                EventType = typeof(int),
                AggregateType = typeof(SimpleEventSourcedRoot),
            });

    }
}
