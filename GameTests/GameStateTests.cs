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
        gameState.Table.Piles.ForEach(casinoDrawPile => Assert.NotEmpty(casinoDrawPile.Cards));
    }

    [Fact]
    public void SumPoints_AllCardsToOnePlayer_11()
    {
        var gameState = new GameState(["Alex"]);
        var table = new Table();
        var player = gameState.Players[0];
        // setting up table with a fiver
        player.Hand.Add(new StandardPlayingCard(Rank.Five, Suit.Spades));
        player.PlaceCard(table, 0);
        // taking the card on the table with card in hand
        player.Hand.Add(new StandardPlayingCard(Rank.Five, Suit.Diamonds));
        player.Take(table, [0], 0);
        // Hacking an advancement of the turn to make the current player the last to take
        gameState.AdvanceTurn("_take");
        // Adding a full set of cards to player's hand
        player.PointPile.AddRange(new StandardPlayingCardDeck().Cards);
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

    [Fact]
    public void NewGameState_Display_Rank()
    {
        var gameState = new GameState(["Alex", "Nikolaj", "Laust"]);
        var display = gameState.DisplayHand("Alex");
        _testOutputHelper.WriteLine(display);
    }

    [Fact]
    public void SumPoints_TwoPlayersWithEqualAmountsOfSpades_PointsAreNullified()
    {
        var gameState = new GameState(["Alex", "Niko"]);

        // Adding cards to Alex's hand
        gameState.Players[0].Hand.Add(new StandardPlayingCard(Rank.Three, Suit.Spades));
        
        // Adding cards to Niko's pointpile
        gameState.Players[1].PointPile.AddRange([
            new StandardPlayingCard(Rank.Seven, Suit.Spades), new StandardPlayingCard(Rank.Six, Suit.Spades)
        ]);

        // Adding cards to the table for Alex to take
        gameState.Table.AddCards([new StandardPlayingCard(Rank.Three, Suit.Hearts)]);

        // Alex taking Three of hearts with Three of spades
        Assert.True(gameState.Players[0].Take(gameState.Table, [0], 0));
        
        // Alex pointpile: Three of spades, Two of hearts (2 cards)
        // Niko pointpile: Seven of spades, Six of spades (2 cards)

        // Adding a single card to the deck for the sumpoints method to not be called from advance turn
        gameState.Deck.Cards.Push(new StandardPlayingCard(Rank.Eight, Suit.Hearts));

        // Advancing turn - for Alex to be the last to take
        gameState.AdvanceTurn("_take");

        var results = gameState.SumPoints();

        foreach (var result in results)
        {
            _testOutputHelper.WriteLine($"{result.Item1.Name} : {result.Item2}");
        }
    }
}