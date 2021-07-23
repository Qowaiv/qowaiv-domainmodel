using ConquerClub.Domain;
using ConquerClub.Domain.Events;
using ConquerClub.Domain.Handlers;
using Qowaiv.DomainModel;
using Qowaiv.Validation.Abstractions;
using System.Collections.Generic;
using Troschuetz.Random.Generators;
using CountryId = Qowaiv.Identifiers.Id<ConquerClub.Domain.ForCountry>;
using GameId = Qowaiv.Identifiers.Id<ConquerClub.Domain.ForGame>;

namespace ConquerClub.UnitTests
{
    internal static class Arrange
    {
        public static readonly GameId GameId = GameId.Parse("test_game");
        public static readonly CountryId Netherlands = CountryId.Create(0);
        public static readonly CountryId Belgium = CountryId.Create(1);
        public static readonly CountryId Luxembourg = CountryId.Create(2);
        public static readonly CountryId Unknown = CountryId.Create(666);

        public static Result<Game> Handle(dynamic command, EventBuffer<GameId> buffer = null)
        {
            buffer ??= new EventBuffer<GameId>(GameId);
            var handler = new TestHandler(buffer, 17);
            var result = handler.Handle(command);
            return result.IsValid
                ? Result.For(handler.Buffer.Load(), result.Messages)
                : Result.WithMessages<Game>(result.Messages);
        }

        public static EventBuffer<GameId> Benelux(int roundLimit = 10) =>
            BeneluxWithoutArmies(roundLimit)
            .Add(new ArmiesInitialized
            {
                Armies = new[] 
                { 
                    Player.P1.Army(3),
                    Player.P2.Army(3),
                    Player.Neutral.Army(3),
                }
            })
            .Add(new TurnStarted(Player.P1.Army(3)));

        public static EventBuffer<GameId> BeneluxWithoutArmies(int roundLimit = 10) =>
            new EventBuffer<GameId>(GameId)
            .Add(new SettingsInitialized
            {
                Players = 2,
                RoundLimit = roundLimit,
            })
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

        public static EventBuffer<GameId> Deploy(this EventBuffer<GameId> game) =>
            game.Add(new Deployed
            {
                Army = Player.P1.Army(3),
                Country = Netherlands,
            });

        public static Game Load(this EventBuffer<GameId> buffer) =>
            AggregateRoot.FromStorage<Game, GameId>(buffer.MarkAllAsCommitted());
    }

    internal class TestHandler
    {
        public TestHandler(EventBuffer<GameId> buffer, int seed)
        {
            rnd = new MT19937Generator(seed);
            Buffer = buffer;
        }

        public Result Handle(dynamic command)
        {
            var uncommited = new List<object>();
            dynamic handler = new GameCommandHandler(rnd,
                load: id => Buffer.Load(),
                save: game =>
                {
                    uncommited.AddRange(game.Buffer.Uncommitted);
                    return Result.OK;
                });

            var result = handler.Handle(command);
            Buffer = Buffer.Add(uncommited);
            return result;
        }

        public EventBuffer<GameId> Buffer { get; private set; }
        private readonly MT19937Generator rnd;
    }
}
