using ConquerClub.Domain;
using ConquerClub.Domain.Events;
using ConquerClub.Domain.Handlers;
using Qowaiv.DomainModel;
using Qowaiv.Identifiers;
using Qowaiv.Validation.Abstractions;
using Troschuetz.Random.Generators;

namespace ConquerClub.UnitTests
{
    internal static class Arrange
    {
        public static readonly Id<ForGame> GameId = Id<ForGame>.Parse("test_game");
        public static readonly Id<ForCountry> Netherlands = Id<ForCountry>.Create(0);
        public static readonly Id<ForCountry> Belgium = Id<ForCountry>.Create(1);
        public static readonly Id<ForCountry> Luxembourg = Id<ForCountry>.Create(2);
        public static readonly Id<ForCountry> Unknown = Id<ForCountry>.Create(666);

        public static Result<Game> Handle(dynamic command, EventBuffer<Id<ForGame>> buffer = null)
        {
            buffer ??= new EventBuffer<Id<ForGame>>(GameId);
            var handler = new TestHandler(buffer, 17);
            dynamic dyn = (dynamic)handler;
            Result result = dyn.Handle(command);

            return result.IsValid
                ? Result.For(handler.Buffer.Load(), result.Messages)
                : Result.WithMessages<Game>(result.Messages);
        }

        public static EventBuffer<Id<ForGame>> Benelux(int roundLimit = 10) =>
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

        public static EventBuffer<Id<ForGame>> BeneluxWithoutArmies(int roundLimit = 10) =>
            new EventBuffer<Id<ForGame>>(GameId)
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
                            Id<ForCountry>.Create(0),
                            Id<ForCountry>.Create(1),
                            Id<ForCountry>.Create(2),
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

        public static EventBuffer<Id<ForGame>> Deploy(this EventBuffer<Id<ForGame>> game) =>
            game.Add(new Deployed
            {
                Army = Player.P1.Army(3),
                Country = Netherlands,
            });

        public static Game Load(this EventBuffer<Id<ForGame>> buffer) =>
            AggregateRoot.FromStorage<Game, Id<ForGame>>(buffer.MarkAllAsCommitted());
    }

    internal class TestHandler : GameCommandHandler
    {
        public TestHandler(EventBuffer<Id<ForGame>> buffer, int seed)
            : base(
                new MT19937Generator(seed),
                id =>
                {
                    return buffer.Load();
                },
                game => 
                {
                    buffer.AddRange(game.Buffer.Uncommitted); 
                    return Result.OK; 
                })
        {
            Buffer = buffer;
        }

        public EventBuffer<Id<ForGame>> Buffer { get; }
    }
}
