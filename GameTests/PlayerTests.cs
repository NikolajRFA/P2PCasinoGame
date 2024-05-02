using Game;
using Xyaneon.Games.Cards;
using Xyaneon.Games.Cards.StandardPlayingCards;

namespace TestProject1;

public class PlayerTests
{
    [Fact]
    public void Take_Table5WithHand5_True()
    {
        var table = new Table();
        var player = new Player { Name = "Alex" };
        player.Hand.Add(new StandardPlayingCard(Rank.Five, Suit.Hearts));
        player.PlaceCard(table, 0);
        player.Hand.Add(new StandardPlayingCard(Rank.Five, Suit.Diamonds));

        Assert.True(player.Take(table, [0], 0));
    }

    [Fact]
    public void Take_Table5WithHand6_False()
    {
        var table = new Table();
        var player = new Player { Name = "Alex" };
        player.Hand.Add(new StandardPlayingCard(Rank.Five, Suit.Hearts));
        player.PlaceCard(table, 0);
        player.Hand.Add(new StandardPlayingCard(Rank.Six, Suit.Diamonds));

        Assert.False(player.Take(table, [0], 0));
    }

    [Fact]
    public void Take_Table10And4WithHandAce_True()
    {
        var table = new Table();
        var player = new Player();
        // Hack some cards onto the table
        player.Hand.AddRange([new StandardPlayingCard(Rank.Ten, Suit.Clubs), new StandardPlayingCard(Rank.Four, Suit.Diamonds)]);
        player.PlaceCard(table, 0);
        player.PlaceCard(table, 0);
        // Setup player for take
        player.Hand.Add(new StandardPlayingCard(Rank.Ace, Suit.Hearts));
        Assert.True(player.Take(table, [0, 1], 0));
    }

    [Fact]
    public void Take_Table11Plus11WithHand11_True()
    {
        var table = new Table();
        var player = new Player();
        // Hack some cards onto the table
        player.Hand.AddRange([new StandardPlayingCard(Rank.Jack, Suit.Clubs), new StandardPlayingCard(Rank.Jack, Suit.Diamonds)]);
        player.PlaceCard(table, 0);
        player.PlaceCard(table, 0);
        // Setup player for take
        player.Hand.Add(new StandardPlayingCard(Rank.Jack, Suit.Spades));
        Assert.True(player.Take(table, [0, 1], 0));
    }

    [Fact]
    public void ClearTable_PlayerPlays5OfSpades_True()
    {
        var table = new Table();
        var player = new Player();
        player.Hand.Add(new StandardPlayingCard(Rank.Five, Suit.Clubs));
        player.PlaceCard(table, 0);
        player.Hand.Add(new StandardPlayingCard(Rank.Five, Suit.Spades));

        Assert.True(player.ClearTable(table, 0));
    }

    [Fact]
    public void ClearTable_PlayerClearsAnEmptyTable_True()
    {
        var table = new Table();
        var player = new Player();
        player.Hand.Add(new StandardPlayingCard(Rank.Five, Suit.Spades));

        Assert.True(player.ClearTable(table, 0));
        Assert.Equal(1, player.ClearCount);
    }
    
}