namespace IEvangelist.Blazing.WarFleet
{
    public record PlayerShotResult(
        ServerGame Server,
        bool IsHit,
        bool IsSunk,
        string? ShipName = null);
}
