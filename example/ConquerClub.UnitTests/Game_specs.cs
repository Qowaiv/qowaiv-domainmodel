using ConquerClub.Domain;
using FluentAssertions;
using NUnit.Framework;
using Qowaiv.Validation.Abstractions;
using Qowaiv.Validation.TestTools;
using System.Linq;
using static ConquerClub.UnitTests.Arrange;
using Commands = ConquerClub.Domain.Commands;

namespace Game_specs
{
    public class Start_Game
    {
        [Test]
        public void With_two_players_includes_third_neutral_player()
        {
            var command = new Commands.Start(
                Game: GameId,
                Players: 2,
                RoundLimit: 10,
                Continents: new[]
                {
                    new Commands.Continent(
                        Name: "Benelux",
                        Bonus: 3,
                        Territories: new []
                        {
                            Netherlands,
                            Belgium,
                            Luxembourg,
                        }),
                },
                Countries: new[]
                {
                    new Commands.Country("Netherlands", new []{ Belgium }),
                    new Commands.Country("Belgium", new []{ Netherlands, Luxembourg }),
                    new Commands.Country("Luxembourg", new []{ Belgium }),
                });

            Handle(command).Should().BeValid()
                .Value.Countries.Select(c => c.Owner).Should().BeEquivalentTo(new[]
            {
                Player.P1,
                Player.P2,
                Player.Neutral
            });
        }
    }

    public class Deploy
    {
        [Test]
        public void Can_not_be_applied_by_not_active_player()
        {
            var command = new Commands.Deploy(
                Luxembourg,
                Player.P2.Army(3),
                GameId,
                ExpectedVersion: 4);

            Handle(command, Benelux()).Should().BeInvalid()
                .WithMessage(ValidationMessage.Error("Action can only be applied by the active P1, not by P2."));
        }

        [Test]
        public void Can_be_applied_by_active_player()
        {
            var command = new Commands.Deploy(
                Netherlands,
                Player.P1.Army(3),
                GameId,
                ExpectedVersion: 4);

            Handle(command, Benelux()).Should().BeValid()
                .Value.Countries.ById(Netherlands).Army.Should().Be(Player.P1.Army(6));
        }

        [Test]
        public void Can_only_be_applied_on_own_country()
        {
            var command = new Commands.Deploy(
                Luxembourg,
                Player.P1.Army(3),
                GameId,
                ExpectedVersion: 4);

            Handle(command, Benelux()).Should().BeInvalid()
                .WithMessage(ValidationMessage.Error("Country Luxembourg must be owned by P1."));
        }
    }
}
