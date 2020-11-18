using Qowaiv.Identifiers;

namespace ConquerClub.Domain.Commands
{
    public class Start : Command
    {
        public Country[] Countries { get; set; }
        public Continent[] Continents { get; set; }
        public int RoundLimit { get; set; }
        public int Players { get; set; }

        public class Country
        {
            public string Name { get; set; }
            public Id<ForCountry>[] Borders { get; set; }
        }

        public class Continent
        {
            public string Name { get; set; }
            public int Bonus { get; set; }
            public Id<ForCountry>[] Territories { get; set; }
        }
    }
}
