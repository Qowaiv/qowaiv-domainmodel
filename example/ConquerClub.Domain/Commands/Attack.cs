using Qowaiv.Identifiers;

namespace ConquerClub.Domain.Commands
{
    public class Attack : Command
    {
        public Id<ForCountry> Attacker { get; set; }
        public Id<ForCountry> Defender { get; set; }
    }
}
