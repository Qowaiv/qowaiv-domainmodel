namespace ConquerClub.Domain
{
    /// <summary>Represents the outcome of an attack.</summary>
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
        public override string ToString() => $"Attacker: {Attacker}, Defender: {Defender}";
    }
}
