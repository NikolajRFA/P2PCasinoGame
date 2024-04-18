using Game;
using Xunit.Abstractions;
using Xyaneon.Games.Cards.StandardPlayingCards;

namespace TestProject1;

public class GameStateTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public GameStateTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Constructor_3Players_3PlayersReceive4CardsEachAndTableHas4Cards()
    {
        var gameState = new GameState(["Alex", "Nikolaj", "Laust"]);
        gameState.Players.ForEach(player =>
        {
            _testOutputHelper.WriteLine($"Player: {player.Name} has cards {string.Join(" ,", player.Hand)}");
            Assert.True(player.Hand.Count == 4);
        });
        gameState.Table.Cards.ForEach(kvp =>
        {
            Assert.NotEmpty(kvp.Key.Cards);
        });
    }

    [Fact]
    public void SumPoints_AllCardsToOnePlayer_11()
    {
        var gameState = new GameState(["Alex"]);
        gameState.Players[0].PointPile.AddRange(new StandardPlayingCardDeck().Cards);
        Assert.Equal(11, gameState.SumPoints()[0].Item2);
    }
}