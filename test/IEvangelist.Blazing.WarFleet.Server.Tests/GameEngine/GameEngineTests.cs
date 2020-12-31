using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace IEvangelist.Blazing.WarFleet.Tests
{
    public class GameEngineTests
    {
        readonly HashSet<Ship> _playerOneShips = new();
        readonly HashSet<Ship> _playerTwoShips = new();

        public GameEngineTests()
        {
            /* Player 1 Layout:
              1 2 3 4 5 6 7 8 9 10
            A C B B B B
            B C D S S S
            C C D
            D C D
            E C     P P
            F
            G
            H
            I
            J
            */
            _playerOneShips.Add(new Carrier(new('A', 1), ShipAlignment.Vertical));
            _playerOneShips.Add(new Battleship(new('A', 2)));
            _playerOneShips.Add(new Destroyer(new('B', 2), ShipAlignment.Vertical));
            _playerOneShips.Add(new Submarine(new('B', 3)));
            _playerOneShips.Add(new PatrolBoat(new('E', 4)));

            /* Player 2 Layout:
              1 2 3 4 5 6 7 8 9 10
            A
            B
            C
            D
            E       S S S
            F       B           C
            G       B           C
            H   D   B           C
            I   D   B       P   C
            J   D           P   C
            */
            _playerTwoShips.Add(new Carrier(new('F', 10), ShipAlignment.Vertical));
            _playerTwoShips.Add(new Battleship(new('F', 4), ShipAlignment.Vertical));
            _playerTwoShips.Add(new Destroyer(new('H', 2), ShipAlignment.Vertical));
            _playerTwoShips.Add(new Submarine(new('E', 4)));
            _playerTwoShips.Add(new PatrolBoat(new('I', 8), ShipAlignment.Vertical));
        }

        public static IEnumerable<object[]> ProcessPlayerMoveInputs()
        {
            yield return new object[] { 'H', 10, true };
            yield return new object[] { 'E', 10, false };
            yield return new object[] { 'D', 5, false };
            yield return new object[] { 'F', 3, false };
            yield return new object[] { 'G', 4, true };
            yield return new object[] { 'F', 10, true };
            yield return new object[] { 'H', 9, false };
            yield return new object[] { 'I', 1, false };
            yield return new object[] { 'J', 9, false };
        }

        [Theory, MemberData(nameof(ProcessPlayerMoveInputs))]
        public async ValueTask ProcessPlayerShotAsyncTest(char row, byte col, bool expectedAsHit)
        {
            TestGame testGame = new(_playerOneShips, _playerTwoShips);

            var mock = new Mock<IRepository<ServerGame>>();
            var taskWrappedGame = Task.FromResult<ServerGame>(testGame);
            var valueTask = new ValueTask<ServerGame>(taskWrappedGame);
            mock.Setup(repo => repo.GetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(valueTask);
            mock.Setup(repo => repo.GetAsync(It.IsAny<string>(), It.IsAny<PartitionKey>(), It.IsAny<CancellationToken>())).Returns(valueTask);
            mock.Setup(repo => repo.UpdateAsync(It.IsAny<ServerGame>(), It.IsAny<CancellationToken>())).Returns(valueTask);

            var engine = new GameEngineService(mock.Object);

            var result = await engine.ProcessPlayerShotAsync("1", testGame.Game.PlayerOne.Id, new(row, col));
            Assert.Equal(expectedAsHit, result.IsHit);
            Assert.Equal(nameof(Carrier), result.ShipName);
            Assert.Contains(new(new(row, col), true), testGame.Game.PlayerOne.ShotsFired);
            Assert.Equal(GameResult.Active, testGame.Game.Result);
        }
    }

    public class TestGame : ServerGame
    {
        public TestGame(HashSet<Ship> playerOneShips, HashSet<Ship> playerTwoShips) => Game = new()
        {
            PlayerOne = new("Player 1") { Ships = playerOneShips },
            PlayerTwo = new("Player 2") { Ships = playerTwoShips }
        };
    }
}