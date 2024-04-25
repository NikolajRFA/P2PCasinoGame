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

        Assert.True(player.Take(table, 0, 0));
    }

    [Fact]
    public void Take_Table5WithHand6_False()
    {
        var table = new Table();
        var player = new Player { Name = "Alex" };
        player.Hand.Add(new StandardPlayingCard(Rank.Five, Suit.Hearts));
        player.PlaceCard(table, 0);
        player.Hand.Add(new StandardPlayingCard(Rank.Six, Suit.Diamonds));

        Assert.False(player.Take(table, 0, 0));
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
    public void BuildTable_4Plus4Equals8_True()
    {
        var table = new Table();
        var player = new Player();
        player.Hand.Add(new StandardPlayingCard(Rank.Four, Suit.Hearts));
        player.PlaceCard(table, 0);
        player.Hand.Add(new StandardPlayingCard(Rank.Four, Suit.Diamonds));
        player.PlaceCard(table, 0);
        Assert.True(player.BuildTable(table, 0, 1, 8));
    }

    [Fact]
    public void BuildTable_4Plus4As8Plus2Equals10_True()
    {
        var table = new Table();
        var player = new Player();
        var pile = new DrawPile<StandardPlayingCard>(isFaceUp: true);
        pile.PlaceOnTop(new StandardPlayingCard(Rank.Four, Suit.Clubs));
        pile.PlaceOnTop(new StandardPlayingCard(Rank.Four, Suit.Diamonds));
        table.Cards.Add(new KeyValuePair<DrawPile<StandardPlayingCard>, List<int>>(pile, [8]));
        player.Hand.Add(new StandardPlayingCard(Rank.Two, Suit.Clubs));
        player.PlaceCard(table, 0);
        Assert.True(player.BuildTable(table, 0, 1, 10));
    }
}