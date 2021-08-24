using System;
using System.Text.RegularExpressions;

namespace ConquerClub.Domain
{
    /// <summary>Represents an <see cref="Army"/></summary>
    /// <remarks>
    /// Equals is implemented including the owner (useful for unit tests).
    /// The == and != use (like, <, >, <=, >=) <see cref="IComparable{Army}"/>
    /// to allow the comparison of the size of the army only.
    /// </remarks>
    public readonly struct Army : IEquatable<Army>, IComparable<Army>, IComparable<int>, IFormattable
    {
        private static readonly Regex Pattern = new(@"^(?<Player>[A-Z0-9]+)\.Army\((?<Size>[0-9]+)\)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>Gets no army.</summary>
        public static readonly Army None;

        /// <summary>Creates a new army.</summary>
        internal Army(Player owner, int size)
        {
            Owner = owner;
            Size = size;
        }

        /// <summary>Gets the player who owns the army.</summary>
        public readonly Player Owner;

        /// <summary>Gets the size of the army.</summary>
        public readonly int Size;

        /// <summary>Adds the other army to this army.</summary>
        public Army Add(Army other)
            => other.Size == 0
            ? this
            : new Army(Owner, Size + other.Size);

        /// <summary>Subtract the other army from this army.</summary>
        public Army Subtract(Army other)
            => other.Size == 0
            ? this
            : Subtract(other.Size);

        /// <summary>Reduces the size of the army with the total of losses.</summary>
        public Army Subtract(int losses)
        {
            var size = Size - losses;
            return size == 0
                ? None
                : Owner.Army(size);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is Army other && Equals(other);

        /// <inheritdoc/>
        public bool Equals(Army other) => Owner == other.Owner && Size == other.Size;

        /// <inheritdoc/>
        public int CompareTo(Army other) => Size.CompareTo(other.Size);

        /// <inheritdoc/>
        public int CompareTo(int other) => Size.CompareTo(other);

        /// <inheritdoc/>
        public override int GetHashCode() => (Size << 8) | Owner.GetHashCode();

        /// <summary>Represents the <see cref="Army"/> as a <see cref="string"/>.</summary>
        public override string ToString() => ToString(null, null);

        /// <summary>Represents the <see cref="Army"/> as a <see cref="string"/>.</summary>
        public string ToString(string format, IFormatProvider formatProvider) => Size == 0 ? nameof(None) : $"{Owner}.Army({Size})";

        /// <summary>Adds the right army to the left army.</summary>
        public static Army operator +(Army l, Army r) => l.Add(r);

        /// <summary>Subtracts the right army from the left army.</summary>
        public static Army operator -(Army l, Army r) => l.Subtract(r);

        /// <summary>Subtracts the losses from the army.</summary>
        public static Army operator -(Army army, int losses) => army.Subtract(losses);

        /// <summary>Compares the left army size with right army size.</summary>
        public static bool operator ==(Army l, Army r) => l.CompareTo(r) == 0;

        /// <summary>Compares the left army size with right army size.</summary>
        public static bool operator !=(Army l, Army r) => l.CompareTo(r) != 0;

        /// <summary>Compares the left army size with right army size.</summary>
        public static bool operator <(Army l, Army r) => l.CompareTo(r) < 0;

        /// <summary>Compares the left army size with right army size.</summary>
        public static bool operator >(Army l, Army r) => l.CompareTo(r) > 0;

        /// <summary>Compares the left army size with right army size.</summary>
        public static bool operator <=(Army l, Army r) => l.CompareTo(r) <= 0;

        /// <summary>Compares the left army size with right army size.</summary>
        public static bool operator >=(Army l, Army r) => l.CompareTo(r) >= 0;

        /// <summary>Compares the army size.</summary>
        public static bool operator ==(Army army, int size) => army.CompareTo(size) == 0;

        /// <summary>Compares the army size.</summary>
        public static bool operator !=(Army army, int size) => army.CompareTo(size) != 0;

        /// <summary>Compares the army size.</summary>
        public static bool operator <(Army army, int size) => army.CompareTo(size) < 0;

        /// <summary>Compares the army size.</summary>
        public static bool operator >(Army army, int size) => army.CompareTo(size) > 0;

        /// <summary>Compares the army size.</summary>
        public static bool operator <=(Army army, int size) => army.CompareTo(size) <= 0;

        /// <summary>Compares the army size.</summary>
        public static bool operator >=(Army army, int size) => army.CompareTo(size) >= 0;

        /// <summary>Parses the <see cref="string"/> representing the army.</summary>
        public static Army Parse(string str)
        {
            if (string.IsNullOrEmpty(str) || nameof(None).Equals(str, StringComparison.InvariantCultureIgnoreCase))
            {
                return None;
            }
            var match = Pattern.Match(str);

            if (match.Success &&
                int.TryParse(match.Groups[nameof(Size)].Value, out var size) &&
                size > 0)
            {
                return Player.Parse(match.Groups[nameof(Player)].Value).Army(size);
            }
            //throw Error.NoArmyString(str);
            throw new FormatException();
        }

        public static Army FromJson(string str) => Parse(str);
    }
}
