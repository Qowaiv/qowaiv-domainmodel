namespace ConquerClub.Domain
{
    public class Settings
    {
        public Settings(int players, int roundLimit, bool fogOfWar)
        {
            Players = players;
            RoundLimit = roundLimit;
            FogOfWar = fogOfWar;
        }

        public int Players { get; }
        public int RoundLimit { get; }
        public bool FogOfWar { get; }
    }
}
