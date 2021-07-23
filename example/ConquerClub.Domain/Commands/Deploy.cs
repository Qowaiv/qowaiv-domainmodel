using Qowaiv.Identifiers;

namespace ConquerClub.Domain.Commands
{
    public class Deploy : Command
    {
        /// <summary>The country to deploy to.</summary>
        public Id<ForCountry> Country { get; set; }

        /// <summary>The army to deploy.</summary>
        public Army Army { get; set; }
    }
}
