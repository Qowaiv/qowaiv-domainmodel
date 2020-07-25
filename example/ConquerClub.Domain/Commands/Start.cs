using Qowaiv.Identifiers;
using System.Collections.Generic;

namespace ConquerClub.Domain.Commands
{
    public class Start : Command
    {
        public string Title { get; set; }
        public Country[] Countries { get; set; }
        public Continent[] Continents { get; set; }
        public int RoundLimit { get; internal set; }
        public int Players { get; internal set; }

        public class Country
        {
            public string Name { get; set; }
            public int[] Borders { get; set; }
            public int Neutal { get; set; }
        }

        public class Continent
        {
            public string Name { get; set; }
            public int Bonus { get; set; }
            public int[] Territories { get; set; }
        }
    }
}
