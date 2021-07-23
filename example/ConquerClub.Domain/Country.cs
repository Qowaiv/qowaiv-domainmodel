using Qowaiv.Identifiers;
using System.Collections.Generic;
using System.Linq;

namespace ConquerClub.Domain
{
    /// <summary>Represents a region/country/territory.</summary>
    public sealed class Country
    {
        internal Country(Id<ForCountry> id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>Gets the continent of the region.</summary>
        public Continent Continent { get; internal set; }

        public IReadOnlyList<Country> Borders { get; internal set; }

        /// <summary>Gets the identifier of the region.</summary>
        public Id<ForCountry> Id { get; }

        /// <summary>Gets the name of the region.</summary>
        public string Name { get; }

        /// <summary>Gets the owner of the region.</summary>
        public Player Owner => Army.Owner;

        /// <summary>Gets or sets the army occupying the region.</summary>
        public Army Army { get; internal set; }

        /// <inheritdoc/>
        public override string ToString() => $"{Name} ({Id}), Army: {Army}";
    }

    public static class CountryExtensions
    {
        public static Country ById(this IEnumerable<Country> countries, Id<ForCountry> id)
            => countries.FirstOrDefault(c => c.Id == id);

        public static IEnumerable<Player> ActivePlayers(this IEnumerable<Country> countries)
            => countries.Select(c => c.Owner)
            .Distinct()
            .Where(p => p != Player.Neutral)
            .OrderBy(p => p);
    }
}
