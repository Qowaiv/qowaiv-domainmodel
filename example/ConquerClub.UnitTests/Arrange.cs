using ConquerClub.Domain;
using ConquerClub.Domain.Events;
using ConquerClub.Domain.Handlers;
using Qowaiv.DomainModel;
using Qowaiv.Validation.Abstractions;
using Troschuetz.Random.Generators;
using CountryId = Qowaiv.Identifiers.Id<ConquerClub.Domain.ForCountry>;
using GameId = Qowaiv.Identifiers.Id<ConquerClub.Domain.ForGame>;
using Buffer = Qowaiv.DomainModel.EventBuffer<Qowaiv.Identifiers.Id<ConquerClub.Domain.ForGame>>;

namespace ConquerClub.UnitTests
{
    internal static class Arrange
    {
        public static readonly GameId GameId = GameId.Parse("test_game");
        public static readonly CountryId Netherlands = CountryId.Create(0);
        public static readonly CountryId Belgium = CountryId.Create(1);
        public static readonly CountryId Luxembourg = CountryId.Create(2);
        public static readonly CountryId Unknown = CountryId.Create(666);

        public static Result<Game> Handle(dynamic command, Buffer buffer = null)
        {
            buffer ??= new Buffer(GameId);
            var handler = new TestHandler(buffer, 17);
            var result = handler.Handle(command);
            return result.IsValid
                ? Result.For(handler.Buffer.Load(), result.Messages)
                : Result.WithMessages<Game>(result.Messages);
        }

        public static Buffer Benelux(int roundLimit = 10)
            => BeneluxWithoutArmies(roundLimit)
            .Add(new ArmiesInitialized(
                Player.P1.Army(3),
                Player.P2.Army(3),
                Player.Neutral.Army(3)))
            .Add(new TurnStarted(Player.P1.Army(3)));

        public static Buffer BeneluxWithoutArmies(int roundLimit = 10)
            => new Buffer(GameId)
            .Add(new SettingsInitialized(2, roundLimit, false))
            .Add(new MapInitialized
            {
                Continents = new[]
                {
                    new MapInitialized.Continent
                    {
                        Name = "Benelux",
                        Bonus = 3,
                        Territories = new []
                        {
                            CountryId.Create(0),
                            CountryId.Create(1),
                            CountryId.Create(2),
                        }
                    },
                },
                Countries = new[]
                {
                    new MapInitialized.Country
                    {
                        Name = "Netherlands",
                        Borders = new []{ Belgium },
                    },
                    new MapInitialized.Country
                    {
                        Name = "Belgium",
                        Borders = new []{ Netherlands, Luxembourg },
                    },
                    new MapInitialized.Country
                    {
                        Name = "Luxembourg",
                        Borders = new []{ Belgium },
                    },
                }
            });

        public static Buffer Deploy(this Buffer game) =>
            game.Add(new Deployed(Netherlands, Player.P1.Army(3)));

        public static Game Load(this Buffer buffer) =>
            AggregateRoot.FromStorage<Game, GameId>(buffer.MarkAllAsCommitted());
    }

    internal class TestHandler
    {
        public TestHandler(Buffer buffer, int seed)
        {
            rnd = new MT19937Generator(seed);
            Buffer = buffer;
        }

        public Result Handle(dynamic command)
        {
            dynamic handler = new GameCommandHandler(rnd,
                load: id => Buffer.Load(),
                save: game =>
                {
                    Buffer = game.Buffer;
                    return Result.OK;
                });

            var result = handler.Handle(command);
            return result;
        }

        public Buffer Buffer { get; private set; }
        private readonly MT19937Generator rnd;
    }
}
