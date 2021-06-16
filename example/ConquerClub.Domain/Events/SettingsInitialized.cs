namespace ConquerClub.Domain.Events
{
    public class SettingsInitialized
    {
        public int Players { get; set; }
        public int RoundLimit { get; set; }
        public bool FogOfWar { get; set; }
    }
}
