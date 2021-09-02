using CountryId = Qowaiv.Identifiers.Id<ConquerClub.Domain.ForCountry>;

namespace ConquerClub.Domain.Events
{
    public record ArmyInitiated(CountryId Country, Army Army);
}
