using System.Text.Json;
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
            _testOutputHelper.WriteLine(
                $"Player: {player.Name} has cards {string.Join(", ", player.Hand.Select(card => $"{card.Rank} of {card.Suit}"))}");
            Assert.True(player.Hand.Count == 4);
        });
        gameState.Table.Cards.ForEach(kvp => Assert.NotEmpty(kvp.Key.Cards));
    }

    [Fact]
    public void SumPoints_AllCardsToOnePlayer_11()
    {
        var gameState = new GameState(["Alex"]);
        gameState.Players[0].PointPile.AddRange(new StandardPlayingCardDeck().Cards);
        Assert.Equal(11, gameState.SumPoints()[0].Item2);
    }

    [Fact]
    public void SerializeDeserialize_GameState_EqualsAfterDeserialization()
    {
        var gameState = new GameState(["Alex", "Nikolaj"]);
        var json = gameState.Serialize();
        var gameStateDeserialized = GameState.Deserialize(json);
        Assert.Equal(gameState.Serialize(), gameStateDeserialized.Serialize());
    }

    [Fact]
    public void Reverse_StandardDeck_Success()
    {
        var deck1 = new StandardPlayingCardDeck();
        var deck2 = new StandardPlayingCardDeck();
        deck2.Reverse();
        Assert.NotEqual(deck1.Draw(), deck2.Draw());
    }
}