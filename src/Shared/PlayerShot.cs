namespace IEvangelist.Blazing.WarFleet.Shared
{
    public record PlayerShot(
        GameResult GameResult,
        string PlayerId,
        Position Shot,
        bool IsHit,
        bool IsSunk,
        string? ShipName);
}
