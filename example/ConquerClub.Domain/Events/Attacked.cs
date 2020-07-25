using Qowaiv.Identifiers;

namespace ConquerClub.Domain.Events
{
    public class Attacked
    {
        public Id<ForCountry> From { get; set; }
        public Id<ForCountry> To { get; set; }

        public Army Attacker { get; set; }
        public Army Defender { get; set; }
        public Army Buffer { get; set; }
    }
}
