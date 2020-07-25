using Qowaiv.Identifiers;

namespace ConquerClub.Domain.Commands
{
    public class Reinforce : Command
    {
        /// <summary>The country to reinforce from.</summary>
        public Id<ForCountry> From { get; set; }

        /// <summary>The country to reinforce to.</summary>
        public Id<ForCountry> To { get; set; }

        /// <summary>The army to add.</summary>
        public Army Army { get; set;  }
    }
}
