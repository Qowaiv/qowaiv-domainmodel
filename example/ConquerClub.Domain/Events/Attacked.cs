using Qowaiv.Identifiers;

namespace ConquerClub.Domain.Events
{
    public class Attacked
    {
        public Id<ForCountry> Attacker { get; set; }
        public Id<ForCountry> Defender { get; set; }

        public AttackResult Result { get; set; }
    }
}
