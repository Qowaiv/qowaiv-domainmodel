using ConquerClub.Domain;
using ConquerClub.Domain.Commands;
using NUnit.Framework;
using Qowaiv.DomainModel;
using Qowaiv.Identifiers;
using Qowaiv.Validation.TestTools;
using System.Linq;
using static ConquerClub.UnitTests.Arrange;

namespace Game_specs
{
    public class Start_Game
    {
        [Test]
        public void With_two_players_includes_thrid_neutral_player()
        {
            var command = new Start
            {
                Players = 2,
                RoundLimit = 10,
                Continents = new[]
                {
                    new Start.Continent
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
                    new Start.Country
                    {
                        Name = "Netherlands",
                        Borders = new []{ Id<ForCountry>.Create(1) },
                    },
                    new Start.Country
                    {
                        Name = "Belgium",
                        Borders = new []{ Id<ForCountry>.Create(0), Id<ForCountry>.Create(2) },
                    },
                    new Start.Country
                    {
                        Name = "Luxembourg",
                        Borders = new []{ Id<ForCountry>.Create(1) },
                    },
                }
            };

            var game = TestHandler(command, new EventBuffer<Id<ForGame>>(GameId));

            ValidationMessageAssert.IsValid(game);

            CollectionAssert.AreEquivalent(new[]
            {
                Player.P1,
                Player.P2,
                Player.Neutral
            },
            game.Value.Countries.Select(c => c.Owner).Distinct());
        }
    }
}
