using CountryId = Qowaiv.Identifiers.Id<ConquerClub.Domain.ForCountry>;

namespace ConquerClub.Domain.Events
{
    public record Reinforced(CountryId From, CountryId To, Army Army);
}
