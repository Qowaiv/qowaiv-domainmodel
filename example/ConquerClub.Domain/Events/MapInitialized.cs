namespace ConquerClub.Domain.Events
{
    public class MapInitialized
    {
        public Country[] Countries { get; set; }
        public Continent[] Continents { get; set; }

        public class Country
        {
            public string Name { get; set; }
            public int[] Borders { get; set; }
        }

        public class Continent
        {
            public string Name { get; set; }
            public int Bonus { get; set; }
            public int[] Territories { get; set; }
        }
    }
}
