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
    public void Build_SingleValuePlusSingleValue_True()
    {
        var table = new Table();
        var player = new Player();
        player.Hand.Add(new StandardPlayingCard(Rank.Two, Suit.Clubs));
        player.PlaceCard(table, 0);
        player.Hand.Add(new StandardPlayingCard(Rank.Three, Suit.Clubs));
        player.Hand.Add(new StandardPlayingCard(Rank.Five, Suit.Diamonds));
        Assert.True(player.Build(table, 0, 0, 5));
    }

    [Fact]
    public void Build_DualValuePlusSingleValue_True()
    {
        var table = new Table();
        var player = new Player();
        player.Hand.Add(new StandardPlayingCard(Rank.Ace, Suit.Clubs));
        player.PlaceCard(table, 0);
        player.Hand.Add(new StandardPlayingCard(Rank.Three, Suit.Diamonds));
        player.Hand.Add(new StandardPlayingCard(Rank.Four, Suit.Hearts));
        Assert.True(player.Build(table, 0, 0, 4));
    }

    [Fact]
    public void Build_SingleValuePlusDualValue_True()
    {
        var table = new Table();
        var player = new Player();
        player.Hand.Add(new StandardPlayingCard(Rank.Three, Suit.Hearts));
        player.PlaceCard(table, 0);
        player.Hand.Add(new StandardPlayingCard(Rank.Ace, Suit.Hearts));
        player.Hand.Add(new StandardPlayingCard(Rank.Four, Suit.Hearts));
        Assert.True(player.Build(table, 0, 0, 4));
    }

    [Fact]
    public void Build_DualValuePlusDualValue_True()
    {
        var table = new Table();
        var player = new Player();
        player.Hand.Add(new StandardPlayingCard(Rank.Ace, Suit.Clubs));
        player.PlaceCard(table, 0);
        player.Hand.Add(new StandardPlayingCard(Rank.Ace, Suit.Hearts));
        player.Hand.Add(new StandardPlayingCard(Rank.Two, Suit.Hearts));
        Assert.True(player.Build(table, 0, 0, 2));
    }

    [Fact]
    public void Build_5Plus5Is12_False()
    {
        var table = new Table();
        var player = new Player();
        player.Hand.Add(new StandardPlayingCard(Rank.Five, Suit.Diamonds));
        player.PlaceCard(table, 0);
        player.Hand.Add(new StandardPlayingCard(Rank.Five, Suit.Clubs));
        Assert.False(player.Build(table, 0, 0, 12));
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