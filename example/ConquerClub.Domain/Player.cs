#pragma warning disable S1210 // "Equals" and the comparison operators should be overridden when implementing "IComparable"
// Players should be sortable, but less than, or greater than has no meaning here.
using Qowaiv;

namespace ConquerClub.Domain;

/// <summary>Represents a player identifier.</summary>
public readonly struct Player : IEquatable<Player>, IComparable<Player>
{
    /// <summary>Represents the neutral <see cref="Player"/>.</summary>
    public static readonly Player Neutral;

    /// <summary>Represents an unknown <see cref="Player"/>.</summary>
    public static readonly Player Unknown = new(byte.MaxValue);

    /// <summary>Gets player P1.</summary>
    public static readonly Player P1 = new(1);

    /// <summary>Gets player P2.</summary>
    public static readonly Player P2 = new(2);

    /// <summary>Gets player P3.</summary>
    public static readonly Player P3 = new(3);

    /// <summary>Creates a new instance of the <see cref="Player"/> struct.</summary>
    public Player(byte id) => Id = id;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly byte Id;

    /// <summary>Creates an army for the player.</summary>
    public Army Army(int size) 
        => size == 0
        ? Domain.Army.None
        : new(this, Guard.Positive(size, nameof(size)));

    public bool IsOther(Player other) => !Equals(other);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Player other && Equals(other);

    /// <inheritdoc/>
    public bool Equals(Player other) => Id == other.Id;

    /// <inheritdoc/>
    public int CompareTo(Player other) => Id.CompareTo(other.Id);

    /// <inheritdoc/>
    public override int GetHashCode() => Id;

    /// <inheritdoc/>
    public override string ToString()
    {
        if (this == Neutral) return nameof(Neutral);
        else if (this == Unknown) return nameof(Unknown);
        else return $"P{Id}";
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
        else
        {
            var num = str.StartsWith("P", StringComparison.InvariantCultureIgnoreCase) ? str[1..] : str;
            return byte.TryParse(num, out var id) ? new(id) : Unknown;
        }
    }

    /// <summary>Serializes the <see cref="Player"/> as JSON string.</summary>
    public string ToJson() => ToString();

    /// <summary>Deserializes the <see cref="Player"/> from a JSON number.</summary>
    public static Player FromJson(long json) => new((byte)json);

    /// <summary>Deserializes the <see cref="Player"/> from a JSON string.</summary>
    public static Player FromJson(string json) => Parse(json);
}
