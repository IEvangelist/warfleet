using System;
using System.Collections.Generic;
using Xunit;

namespace IEvangelist.Blazing.WarFleet.Shared.Tests
{
    public class ModelsExtensionsTests
    {
        public static IEnumerable<object[]> ShipOccupancyInputs()
        {
            yield return new object[]
            {
                new Ship(3, "Destroyer" /*Defaults: new('A', 1), ShipAlignment.Horizontal */),
                new HashSet<Position>
                {
                    new('A', 1), new('A', 2), new('A', 3)
                }
            };
            yield return new object[]
            {
                new Ship(5, "Carrier", new('F', 7), ShipAlignment.Vertical),
                new HashSet<Position>
                {
                    new('F', 7), new('G', 7), new('H', 7), new('I', 7), new('J', 7)
                }
            };
            yield return new object[]
            {
                new Ship(2, "Patrol Boat", new('C', 9), ShipAlignment.Horizontal),
                new HashSet<Position>
                {
                    new('C', 9), new('C', 10)
                }
            };
            yield return new object[]
            {
                new Ship(4, "Battleship", new('C', 5), ShipAlignment.Vertical),
                new HashSet<Position>
                {
                     new('C', 5), new('D', 5), new('E', 5), new('F', 5)
                }
            };
        }

        [Theory, MemberData(nameof(ShipOccupancyInputs))]
        public void GetShipOccupancyTest(Ship ship, HashSet<Position> expectedPositions) =>
            Assert.True(
                HashSet<Position>.CreateSetComparer().Equals(
                    expectedPositions, ship.GetShipOccupancy()),
                "They are not equal.");

        public static IEnumerable<object[]> GamePlayerAndOpponentInputs()
        {
            var knownPlayerId = Guid.NewGuid().ToString();
            var knownOpponentId = Guid.NewGuid().ToString();

            yield return new object[]
            {
                new Game
                {
                    PlayerOne = new("", null, null) { Id = knownPlayerId },
                    PlayerTwo = new("", null, null) { Id = knownOpponentId }
                },
                knownPlayerId,
                knownPlayerId,
                knownOpponentId
            };
            yield return new object[]
            {
                new Game
                {
                    PlayerOne = new("", null, null) { Id = knownPlayerId },
                    PlayerTwo = new("", null, null) { Id = knownOpponentId }
                },
                knownOpponentId,
                knownOpponentId,
                knownPlayerId
            };
        }

        [Theory, MemberData(nameof(GamePlayerAndOpponentInputs))]
        public void GetPlayerAndOpponentTest(Game game, string id, string expectedPlayerId, string expectedOpponentId)
        {
            var (player, opponent) = game.GetPlayerAndOpponent(id);
            Assert.Equal(expectedPlayerId, player.Id);
            Assert.Equal(expectedOpponentId, opponent.Id);
        }
    }
}