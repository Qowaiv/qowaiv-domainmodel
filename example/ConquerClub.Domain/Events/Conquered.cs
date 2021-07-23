using CountryId = Qowaiv.Identifiers.Id<ConquerClub.Domain.ForCountry>;

namespace ConquerClub.Domain.Events
{
    public record Conquered(
        CountryId Attacker,
        CountryId Defender);
}
