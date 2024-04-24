using Game;
using Xunit.Abstractions;
using Xyaneon.Games.Cards.StandardPlayingCards;

namespace TestProject1;

public class DisplayTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public DisplayTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Table_NewGameState_CardsOnTable()
    {
        var gameState = new GameState(["Alex", "Nikolaj", "Laust"]);
        var display = new Display(gameState, "Alex");
        gameState.Table.Cards.First().Key.Cards.Push(new StandardPlayingCard(Rank.Ace, Suit.Clubs));
        _testOutputHelper.WriteLine(display.Table());

    }

    [Fact]
    public void Hand_NewGameState_CardInHand()
    {
        var gameState = new GameState(["Alex", "Nikolaj", "Laust"]);
        var display = new Display(gameState, "Alex");
        _testOutputHelper.WriteLine(display.Hand());
    }
}