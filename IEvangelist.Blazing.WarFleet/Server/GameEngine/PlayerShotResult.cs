namespace IEvangelist.Blazing.WarFleet
{
    public record PlayerShotResult(
        ServerGame Server,
        bool IsHit,
        string? ShipName = null);
}
