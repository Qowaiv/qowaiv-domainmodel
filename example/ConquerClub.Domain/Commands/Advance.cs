using GameId = Qowaiv.Identifiers.Id<ConquerClub.Domain.ForGame>;

namespace ConquerClub.Domain.Commands
{
    public record Advance(Army To, GameId Game, int ExpectedVersion) 
        : Command(Game, ExpectedVersion);
}
