namespace ConquerClub.Domain.Commands;

public record Attack(
    CountryId Attacker,
    CountryId Defender,
    GameId Game,
    int ExpectedVersion)
    : Command(Game, ExpectedVersion);
