namespace IEvangelist.Blazing.WarFleet
{
    public record PlayerShotResult(
        Game Game,
        bool IsHit,
        string? ShipName = null);
}
