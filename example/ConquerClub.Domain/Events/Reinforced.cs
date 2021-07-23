using Qowaiv.Identifiers;

namespace ConquerClub.Domain.Events
{
    public class Reinforced
    {
        public Id<ForCountry> From { get; set; }
        public Id<ForCountry> To { get; set; }
        public Army Army { get; set; }
    }
}
