using Qowaiv.Identifiers;

namespace ConquerClub.Domain.Events
{
    public class ArmyInitiated
    {
        public Id<ForCountry> Country { get; set; }
        public Army Army { get; set; }
    }
}
