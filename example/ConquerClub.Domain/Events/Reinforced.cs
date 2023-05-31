namespace ConquerClub.Domain.Events;

public record Reinforced(CountryId From, CountryId To, Army Army);
