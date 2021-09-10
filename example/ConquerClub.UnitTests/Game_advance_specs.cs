using ConquerClub.Domain;
using ConquerClub.Domain.Commands;
using ConquerClub.Domain.Events;
using FluentAssertions;
using NUnit.Framework;
using Qowaiv.Validation.Abstractions;
using static ConquerClub.UnitTests.Arrange;

namespace Game_specs
{
    public class Advance_can_not_be_applied_when_the
    {
        [Test]
        public void current_phase_is_not_advance()
        {
            var command = new Advance(
                  To: Player.P1.Army(3),
                  Game: GameId,
                  ExpectedVersion: 4);

            Handle(command, Benelux()).Should().BeInvalid()
                .WithMessage(ValidationMessage.Error("Action must be in the Advance phase to be executed, not in the Deploy phase."));
        }

        [Test]
        public void army_of_not_active_player()
        {
            var command = new Advance(
                  To: Player.P2.Army(3),
                  Game: GameId,
                  ExpectedVersion: 5);

            Handle(command, Benelux().Add(new Conquered(Netherlands, Belgium)))
                .Should().BeInvalid()
                .WithMessage(ValidationMessage.Error("Action can only be applied by the active P1, not by P2."));
        }

        [Test]
        public void advancement_exceeds_available([Range(2, 4)]int size)
        {
            var command = new Advance(
                  To: Player.P1.Army(size),
                  Game: GameId,
                  ExpectedVersion: 5);

            Handle(command, Benelux().Add(new Conquered(Netherlands, Belgium)))
                .Should().BeInvalid()
                .WithMessage(ValidationMessage.Error($"Action not be executed as it requires more armies (P1.Army({size})) then available (P1.Army(1))."));
        }
    }

    public class Advance_can_only_be_applied_when
    {
        [Test]
        public void advancement_exceeds_available()
        {
            var command = new Advance(
                  To: Player.P1.Army(1),
                  Game: GameId,
                  ExpectedVersion: 5);

            Handle(command, Benelux().Add(new Conquered(Netherlands, Belgium)))
                .Should().BeValid()
                .Value.ArmyBuffer.Should().Be(Army.None);
        }
    }
}
