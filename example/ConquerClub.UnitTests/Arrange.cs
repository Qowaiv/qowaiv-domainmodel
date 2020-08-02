using ConquerClub.Domain;
using ConquerClub.Domain.Commands;
using ConquerClub.Domain.Handlers;
using Qowaiv.DomainModel.TestTools;
using Qowaiv.Identifiers;
using Qowaiv.Validation.Abstractions;
using Troschuetz.Random;
using Troschuetz.Random.Generators;

namespace ConquerClub.UnitTests
{
    internal static class Arrange
    {
        public static readonly Id<ForGame> GameId = Id<ForGame>.Parse("test_game");

        public static IGenerator Rnd(int seed = 17) => new MT19937Generator(seed);

        public static GameCommandHandler Handler(TestEvents events = null, int seed = 17)
        {
            var rnd = Rnd(seed);
            return new GameCommandHandler
            (
                rnd: rnd,
                load: id => (events ?? TestEvents.Empty).Load<Game, Id<ForGame>>(id),
                save: game => Result.OK
            );
        }

        public static TestEvents Benelux(int players = 3, int roundLimit = 10) =>
            TestEvents.New(new Start
            {
                Players = players,
                RoundLimit = roundLimit,
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
            });
    }
}
