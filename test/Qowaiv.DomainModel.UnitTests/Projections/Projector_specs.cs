using FluentAssertions;
using NUnit.Framework;
using Qowaiv.DomainModel.Projections;

namespace Projections.Projector_specs
{
    public class Projector_specs
    {
        [Test]
        public void X()
        {
            var events = new object[]
            {
                new Dummy(),
                new Number(13),
                new Dummy(),
                new Number(4),
                new Dummy(),
            };
            var projector = new Sum();
            var projection = projector.Project(events);
            projection.Should().Be(17);
        }
    }


    internal class Sum : Projector<int>
    {
        private int sum;

        public int Projection() => sum;

        internal void When(Number number)
        {
            sum += number.Value;
        }
    }


    internal record Dummy();
    internal record Number(int Value);
}
