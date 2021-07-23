using Qowaiv.Identifiers;

namespace ConquerClub.Domain.Commands
{
    public class AutoAttack : Command
    {
        public Id<ForCountry> Attacker { get; set; }
        public Id<ForCountry> Defender { get; set; }
    }
}
