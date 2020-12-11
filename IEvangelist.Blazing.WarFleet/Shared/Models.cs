using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace IEvangelist.Blazing.WarFleet
{
    public record Player(
        string Name,
        TrackingBoard TrackingBoard,
        PlacementBoard PlacementBoard)
    {
        public string Id { get; init; } = Guid.NewGuid().ToString();

        public bool ShipsPlaced => PlacementBoard.Ships.Any();

        public static implicit operator Player(string playerName) =>
            new(playerName, new(Defaults.GameBoard), new(Defaults.GameBoard));
    }

    /// <summary>A ship that takes up five spaces.</summary>
    public record Carrier(
        Position? TopLeftPosition = null,
        ShipAlignment Alignment = default) :
        Ship(5, nameof(Carrier), TopLeftPosition, Alignment);

    /// <summary>A ship that takes up four spaces.</summary>
    public record Battleship(
        Position? TopLeftPosition = null,
        ShipAlignment Alignment = default) :
        Ship(4, nameof(Battleship), TopLeftPosition, Alignment);

    /// <summary>A ship that takes up three spaces.</summary>
    public record Destroyer(
        Position? TopLeftPosition = null,
        ShipAlignment Alignment = default) :
        Ship(3, nameof(Destroyer), TopLeftPosition, Alignment);

    /// <summary>A ship that takes up three spaces.</summary>
    public record Submarine(
        Position? TopLeftPosition = null,
        ShipAlignment Alignment = default) :
        Ship(3, nameof(Submarine), TopLeftPosition, Alignment);

    /// <summary>A ship that takes up two spaces.</summary>
    public record PatrolBoat(
        Position? TopLeftPosition = null,
        ShipAlignment Alignment = default) :
        Ship(2, nameof(PatrolBoat), TopLeftPosition, Alignment);

    public record Ship(
        byte Size,
        string Name,
        Position? TopLeftPosition = null,
        ShipAlignment Alignment = default);

    public record TrackingBoard(HashSet<Position> Cells) : GameBoard(Cells)
    {
        public HashSet<PlayerMove> ShotsFired { get; init; } = new HashSet<PlayerMove>();
    }
    public record PlacementBoard(HashSet<Position> Cells) : GameBoard(Cells)
    {
        public HashSet<Ship> Ships { get; init; } = new HashSet<Ship>();
    }
    public record GameBoard(HashSet<Position> Cells);

    [DebuggerDisplay("{Row}:{Column}")]
    public record Position(char Row, byte Column);

    public record PlayerMove(Position Shot, bool IsHit);

    public enum ShotResult { NotFired, Miss, Hit };
    public enum ShipAlignment { Horizontal, Vertical };
    public enum GameResult { NotYetStarted, Active, PlayerOneWins, PlayerTwoWins };
}
