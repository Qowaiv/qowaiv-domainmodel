namespace ConquerClub.Domain.Events
{
    public record SettingsInitialized(int Players, int RoundLimit, bool FogOfWar);
}
