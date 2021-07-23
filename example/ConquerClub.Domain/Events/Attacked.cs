using CountryId = Qowaiv.Identifiers.Id<ConquerClub.Domain.ForCountry>;

namespace ConquerClub.Domain.Events
{
    public record Attacked(
        CountryId Attacker,
        CountryId Defender,
        AttackResult Result);
}
