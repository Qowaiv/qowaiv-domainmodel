using Buffer = Qowaiv.DomainModel.EventBuffer<Qowaiv.Identifiers.Id<ConquerClub.Domain.ForGame>>;

namespace ConquerClub.UnitTests;

internal static class Arrange
{
    public static readonly GameId Game_Id = GameId.Parse("test_game");
    public static readonly CountryId Netherlands = CountryId.Create(0);
    public static readonly CountryId Belgium = CountryId.Create(1);
    public static readonly CountryId Luxembourg = CountryId.Create(2);
    public static readonly CountryId Unknown = CountryId.Create(666);

    public static Result<Game> Handle(object command, Buffer? buffer = null)
    {
        buffer ??= EventBuffer.Empty(Game_Id);
        var processor = new TestProcessor(buffer, 17);
        var result = processor.Send(command);
        return result.IsValid
            ? Result.For(processor.Buffer.Load(), result.Messages)
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
        => EventBuffer.Empty(Game_Id)
        .Add(new SettingsInitialized(2, roundLimit, false))
        .Add(new MapInitialized(
            Continents: new[] { new ContinentInitialized("Benelux", 3, new[] { Netherlands, Belgium, Luxembourg })},
            Countries: new[]
            {
                new CountryInitialized("Netherlands", new []{ Belgium }),
                new CountryInitialized("Belgium", new []{ Netherlands, Luxembourg }),
                new CountryInitialized("Luxembourg", new []{ Belgium }),
            }));

    public static Buffer Deploy(this Buffer game) =>
        game.Add(new Deployed(Netherlands, Player.P1.Army(3)));

    public static Game Load(this Buffer buffer) =>
        Aggregate.FromStorage<Game, GameId>(buffer.MarkAllAsCommitted());
}

internal class TestProcessor : CommandProcessor<Result>
{
    public TestProcessor(Buffer? buffer, int seed)
    {
        Rnd = new MersenneTwister(seed);
        Buffer = buffer ?? EventBuffer.Empty(GameId.Next());
    }
    
    public Buffer Buffer { get; private set; }
    private MersenneTwister Rnd { get; }

    protected override Type GenericHandlerType => typeof(CommandHandler<>);
    protected override string HandlerMethod => nameof(CommandHandler<object>.Handle);
    protected override object GetHandler(Type handlerType)
        => new GameCommandHandler(Rnd,
            load: id => Buffer.Load(),
            save: game =>
            {
                Buffer = game.Buffer;
                return Result.OK;
            });
}
