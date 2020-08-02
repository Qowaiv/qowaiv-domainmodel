using ConquerClub.Domain;
using NUnit.Framework;
using Qowaiv.Identifiers;
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
        public void With_two_players_includes_thrid_neutral_player()
        {
            var command = new Commands.Start
            {
                Players = 2,
                RoundLimit = 10,
                Continents = new[]
                {
                    new Commands.Start.Continent
                    {
                        Name = "Benelux",
                        Bonus = 3,
                        Territories = new []
                        {
                            Id<ForCountry>.Create(0),
                            Id<ForCountry>.Create(1),
                            Id<ForCountry>.Create(2),
                        }
                    },
                },
                Countries = new[]
                {
                    new Commands.Start.Country
                    {
                        Name = "Netherlands",
                        Borders = new []{ Id<ForCountry>.Create(1) },
                    },
                    new Commands.Start.Country
                    {
                        Name = "Belgium",
                        Borders = new []{ Id<ForCountry>.Create(0), Id<ForCountry>.Create(2) },
                    },
                    new Commands.Start.Country
                    {
                        Name = "Luxembourg",
                        Borders = new []{ Id<ForCountry>.Create(1) },
                    },
                }
            };

            var result = TestHandler(command);

            ValidationMessageAssert.IsValid(result);

            CollectionAssert.AreEquivalent(new[]
            {
                Player.P1,
                Player.P2,
                Player.Neutral
            },
            result.Value.Countries.Select(c => c.Owner).Distinct());
        }
    }

    public class Deploy
    {
        [Test]
        public void Can_only_be_applied_the_active_player()
        {
            var command = new Commands.Deploy
            {
                Army = Player.P2.Army(3),
                Country = Id<ForCountry>.Create(2),
                Game = GameId,
                ExpectedVersion = 4,
            };

            var result = TestHandler(command, Benelux());

            ValidationMessageAssert.WithErrors(result,
                ValidationMessage.Error("Action can only be applied by the active P1, not by P2."));
        }

        [Test]
        public void Can_only_be_applied_on_own_country()
        {
            var command = new Commands.Deploy
            {
                Army = Player.P1.Army(3),
                Country = Id<ForCountry>.Create(2),
                Game = GameId,
                ExpectedVersion = 4,
            };

            var result = TestHandler(command, Benelux());

            ValidationMessageAssert.WithErrors(result,
                ValidationMessage.Error("Country Luxembourg must be owned by P1."));
        }
    }

    public class Attack
    {
        [Test]
        public void Can_only_be_applied_the_active_player()
        {
            var command = new Commands.Attack
            {
                Attacker = Id<ForCountry>.Create(1),
                Defender = Id<ForCountry>.Create(2),
                Game = GameId,
                ExpectedVersion = 4,
            };

            var result = TestHandler(command, Benelux());

            Assert.Inconclusive();

            //ValidationMessageAssert.WithErrors(result,
            //    ValidationMessage.Error("Action can only be applied by the active P1, not by P2."));
        }
    }
}
