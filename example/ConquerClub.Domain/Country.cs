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

        /// <summary>True if the region is 'owned' by <see cref="Player.Neutral"/>.</summary>
        public bool IsNeutal => Owner == Player.Neutral;

        /// <summary>Gets or sets the army occupying the region.</summary>
        public Army Army { get; internal set; }

        /// <inheritdoc/>
        public override string ToString() => $"{Name} ({Id}), Army: {Army}";
    }

    public static class CountryExtensions
    {
        public static Country ById(this IEnumerable<Country> continents, Id<ForCountry> id)
        {
            return continents.FirstOrDefault(c => c.Id == id);
        }
    }
}
