namespace ConquerClub.Domain.Events;

public record Conquered(CountryId Attacker, CountryId Defender);
