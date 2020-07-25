using System;
using System.Diagnostics;

namespace ConquerClub.Domain
{
    /// <summary>Represents the outcome of an attack.</summary>
    [DebuggerDisplay("{DebuggerDisplay}")]
    public readonly struct AttackResult
    {
        public AttackResult(Army attacker, Army defender)
        {
            Attacker = attacker;
            Defender = defender;
        }

        /// <summary>The surviving attackers./summary>
        public Army Attacker { get; }

        /// <summary>The surviving defenders./summary>
        public Army Defender { get; }

        /// <summary>The attack was successful (all defenders where killed).</summary>
        public bool IsSuccess => Defender == Army.None;

        /// <inheritdoc/>
        public override string ToString() => $"{Attacker}:{Defender}";

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"Attacker: {Attacker}, Defender: {Defender}";

        /// <summary>Parses the <see cref="string"/> representing the army.</summary>
        public static AttackResult Parse(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return default;
            }

            var splitted = str.Split(':');

            return splitted.Length == 2
                ? new AttackResult(Army.Parse(splitted[0]), Army.Parse(splitted[1]))
                : throw new FormatException();
        }

        public static AttackResult FromJson(string str) => Parse(str);
    }
}
