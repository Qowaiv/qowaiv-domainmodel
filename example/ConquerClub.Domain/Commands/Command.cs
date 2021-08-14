using GameId = Qowaiv.Identifiers.Id<ConquerClub.Domain.ForGame>;

namespace ConquerClub.Domain.Commands
{
    public record Command(GameId Game, int ExpectedVersion);
}
