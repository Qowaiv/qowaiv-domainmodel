namespace ConquerClub.Domain.Commands;

public record Command(GameId Game, int ExpectedVersion);
