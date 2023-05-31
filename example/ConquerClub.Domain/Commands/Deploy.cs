namespace ConquerClub.Domain.Commands;

public record Deploy(
    CountryId Country,
    Army Army,
    GameId Game,
    int ExpectedVersion)
    : Command(Game, ExpectedVersion);
