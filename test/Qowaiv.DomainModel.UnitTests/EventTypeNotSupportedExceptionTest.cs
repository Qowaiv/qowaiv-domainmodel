//using NUnit.Framework;
//using Qowaiv.DomainModel.UnitTests.Models;
//using Qowaiv.TestTools;

//namespace Qowaiv.DomainModel.UnitTests
//{
//    public class EventTypeNotSupportedExceptionTest
//    {
//        [Test]
//        public void Serialize_RoundTrip()
//        {
//            var exception = new EventTypeNotSupportedException(typeof(int), typeof(SimpleEventSourcedRoot));
//            var actual = SerializationTest.SerializeDeserialize(exception);

//            Assert.AreEqual(typeof(int), actual.EventType);
//            Assert.AreEqual(typeof(SimpleEventSourcedRoot), actual.AggregateType);
//        }
//    }
//}
