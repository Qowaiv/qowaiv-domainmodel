using ConquerClub.Domain;
using ConquerClub.Domain.Commands;
using NUnit.Framework;
using Qowaiv.Validation.Abstractions;
using Qowaiv.Validation.TestTools;
using static ConquerClub.UnitTests.Arrange;

namespace Game_specs
{
    public class Resign_can_not_be_applied_when_the
    {
        [Test]
        public void player_does_not_have_a_country()
        {
            var command = new Resign
            {
                Player = Player.P3,
                ExpectedVersion = 4,
            };

            var result = TestHandler(command, Benelux());

            ValidationMessageAssert.WithErrors(result,
                ValidationMessage.Error(" Player P3 does not have any country."));
        }
    }
}
