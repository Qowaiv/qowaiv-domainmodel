using ConquerClub.Domain.Diagnostics;
using Qowaiv.Identifiers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ConquerClub.Domain
{
    /// <summary>Represents a continent/super-region.</summary>
    [DebuggerTypeProxy(typeof(CollectionDebugView))]
    public sealed class Continent : IEnumerable<Country>
    {
        internal Continent(Id<ForContinent> id, string name, int bonus)
        {
            Id = id;
            Name = name;
            Bonus = bonus;
        }

        /// <summary>Gets the identifier of the continent.</summary>
        public Id<ForContinent> Id { get; }

        /// <summary>Gets the name of the continent.</summary>
        public string Name { get; }

        /// <summary>Gets the bonus for controlling the full continent.</summary>
        public int Bonus { get; }

        /// <summary>Gets the owner of the continent, and <see cref="Player.Neutral"/> if nobody owns the continent.</summary>
        public Player Owner
        {
            get => Countries.All(r => r.Owner == Countries[0].Owner) ? Countries[0].Owner : Player.Neutral;
        }

        /// <summary>Gets the number of regions on the continent.</summary>
        public IReadOnlyList<Country> Countries { get; internal set; }

        /// <inheritdoc/>
        public override string ToString() => $"{Name} ({Id}), bonus {Bonus}, {Countries.Count} countries";

        /// <inheritdoc/>
        public IEnumerator<Country> GetEnumerator() => Countries.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public static class ContinentExtensions
    {
        public static Continent ById(this IEnumerable<Continent> continents, Id<ForContinent> id)
        {
            return continents.FirstOrDefault(c => c.Id == id);
        }
    }
}
