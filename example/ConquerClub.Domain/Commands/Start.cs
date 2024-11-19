namespace ConquerClub.Domain.Commands;

public record Start(
    Country[] Countries,
    Continent[] Continents,
    int RoundLimit,
    int Players,
    GameId Game)
    : Command(Game, 0);

public record Country(string Name, CountryId[] Borders);

public record Continent(string Name, int Bonus, CountryId[] Territories);
