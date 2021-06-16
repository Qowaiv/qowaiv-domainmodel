using System;
using System.Diagnostics;

namespace ConquerClub.Domain
{
    /// <summary>Represents a player identifier.</summary>
    public readonly struct Player : IEquatable<Player>, IComparable<Player>
    {
        /// <summary>Represents the neutral <see cref="Player"/>.</summary>
        public static readonly Player Neutral;

        /// <summary>Represents an unknown <see cref="Player"/>.</summary>
        public static readonly Player Unknown = new Player(byte.MaxValue);

        /// <summary>Gets player P1.</summary>
        public static readonly Player P1 = new Player(1);

        /// <summary>Gets player P2.</summary>
        public static readonly Player P2 = new Player(2);

        /// <summary>Gets player P3.</summary>
        public static readonly Player P3 = new Player(3);

        /// <summary>Creates a new instance of the <see cref="Player"/> struct.</summary>
        public Player(byte id) => Id = id;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly byte Id;

        /// <summary>Creates an army for the player.</summary>
        public Army Army(int size) => new Army(this, size);

        public bool IsOther(Player other) => !Equals(other);

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is Player other && Equals(other);

        /// <inheritdoc/>
        public bool Equals(Player other) => Id == other.Id;

        /// <inheritdoc/>
        public int CompareTo(Player other) => Id.CompareTo(other.Id);

        /// <inheritdoc/>
        public override int GetHashCode() => Id;

        /// <inheritdoc/>
        public override string ToString()
        {
            if (this == Neutral)
            {
                return nameof(Neutral);
            }
            if (this == Unknown)
            {
                return nameof(Unknown);
            }
            return "P" + Id.ToString();
        }

        /// <summary>Returns true if the two <see cref="Player"/>'s are equal.</summary>
        public static bool operator ==(Player l, Player r) => l.Equals(r);

        /// <summary>Returns true if the two <see cref="Player"/>'s are not equal.</summary>
        public static bool operator !=(Player l, Player r) => !(l == r);

        /// <summary>Casts a <see cref="Player"/> to a <see cref="byte"/>.</summary>
        public static explicit operator byte(Player player) => player.Id;

        /// <summary>Parses the player.</summary>
        public static Player Parse(string str)
        {
            if (string.IsNullOrEmpty(str) || nameof(Neutral).Equals(str, StringComparison.InvariantCultureIgnoreCase))
            {
                return Neutral;
            }

            var num = str.StartsWith("P", StringComparison.InvariantCultureIgnoreCase) ? str.Substring(1) : str;

            if (byte.TryParse(num, out var id))
            {
                return new Player(id);
            }
            return Unknown;
        }

        /// <summary>Serializes the <see cref="Player"/> as JSON string.</summary>
        public string ToJson() => ToString();

        /// <summary>Deserializes the <see cref="ContinentId"/> from a JSON number.</summary>
        public static Player FromJson(long json) => new Player((byte)json);

        /// <summary>Deserializes the <see cref="ContinentId"/> from a JSON string.</summary>
        public static Player FromJson(string json) => Parse(json);
    }
}
