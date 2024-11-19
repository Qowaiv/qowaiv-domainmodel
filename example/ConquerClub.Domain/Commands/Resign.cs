namespace ConquerClub.Domain.Commands;

public record Resign(GameId Game, int ExpectedVersion)
    : Command(Game, ExpectedVersion);
