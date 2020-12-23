using Xunit;

namespace IEvangelist.Blazing.WarFleet.Tests
{
    public class GameExtensionsTests
    {
        [Fact]
        public void GetPlayerAndOpponentTest()
        {
            Player lyric = "Lyric";
            Player londyn = "Londyn";
            Game game = new()
            {
                PlayerOne = lyric,
                PlayerTwo = londyn
            };

            var (player, opponent) = game.GetPlayerAndOpponent(lyric.Id);

            Assert.Equal(lyric, player);
            Assert.Equal(londyn.Name, opponent.Name);
        }
    }
}