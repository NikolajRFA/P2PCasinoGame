using Game;

namespace TestProject1;

public class GameStateTests
{
    [Fact]
    public void Constructor_3Players_3PlayersReceive4CardsEachAndTableHas4Cards()
    {
        var gameState = new GameState(["Alex", "Nikolaj", "Laust"]);
        gameState.Players.ForEach(player => Assert.True(player.Hand.Count == 4));
        Assert.True(gameState.Table.Cards.Count == 4);
    }
}