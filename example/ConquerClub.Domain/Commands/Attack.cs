using GameId = Qowaiv.Identifiers.Id<ConquerClub.Domain.ForGame>;
using CountryId = Qowaiv.Identifiers.Id<ConquerClub.Domain.ForCountry>;

namespace ConquerClub.Domain.Commands
{
    public record Attack(
        CountryId Attacker, 
        CountryId Defender,
        GameId Game,
        int ExpectedVersion)
        : Command(Game, ExpectedVersion);
}
