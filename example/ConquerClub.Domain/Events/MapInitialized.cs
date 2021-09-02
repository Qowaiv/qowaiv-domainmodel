using System.Collections.Generic;
using CountryId = Qowaiv.Identifiers.Id<ConquerClub.Domain.ForCountry>;

namespace ConquerClub.Domain.Events
{
    public record MapInitialized(
        IReadOnlyCollection<ContinentInitialized> Continents,
        IReadOnlyCollection<CountryInitialized> Countries);

    public record CountryInitialized(string Name, IReadOnlyCollection<CountryId> Borders);
    public record ContinentInitialized(string Name, int Bonus, IReadOnlyCollection<CountryId> Territories);
}
