using Game;
using Xyaneon.Games.Cards;
using Xyaneon.Games.Cards.StandardPlayingCards;

namespace TestProject1;

public class TableTests
{
    [Fact]
    public void Build_SingleValuePlusSingleValue_True()
    {
        var table = new Table();
        table.AddCard(new StandardPlayingCard(Rank.Two, Suit.Clubs));
        Assert.True(table.Build(0, new StandardPlayingCard(Rank.Three, Suit.Clubs), 5));
    }
    
    [Fact]
    public void Build_DualValuePlusSingleValue_True()
    {
        var table = new Table();
        table.AddCard(new StandardPlayingCard(Rank.Ace, Suit.Clubs));
        Assert.True(table.Build(0, new StandardPlayingCard(Rank.Three, Suit.Diamonds), 4));
    }
    
    [Fact]
    public void Build_SingleValuePlusDualValue_True()
    {
        var table = new Table();
        table.AddCard(new StandardPlayingCard(Rank.Three, Suit.Hearts));
        Assert.True(table.Build(0, new StandardPlayingCard(Rank.Ace, Suit.Hearts), 4));
    }
    
    [Fact]
    public void Build_DualValuePlusDualValue_True()
    {
        var table = new Table();
        table.AddCard(new StandardPlayingCard(Rank.Ace, Suit.Clubs));
        Assert.True(table.Build(0, new StandardPlayingCard(Rank.Ace, Suit.Hearts), 2));
    }

    [Fact]
    public void Build_5Plus5Is12_False()
    {
        var table = new Table();
        table.AddCard(new StandardPlayingCard(Rank.Five, Suit.Diamonds));
        Assert.False(table.Build(0, new StandardPlayingCard(Rank.Five, Suit.Clubs), 12));
    }

    [Fact]
    public void BuildTable_4Plus4Equals8_True()
    {
        var table = new Table();
        table.AddCard(new StandardPlayingCard(Rank.Four, Suit.Hearts));
        table.AddCard(new StandardPlayingCard(Rank.Four, Suit.Diamonds));
        Assert.True(table.BuildTable(0, 1, 8));
    }
    [Fact]
    public void BuildTable_4Plus4As8Plus2Equals10_True()
    {
        var table = new Table();
        var pile = new DrawPile<StandardPlayingCard>(isFaceUp: true);
        pile.PlaceOnTop(new StandardPlayingCard(Rank.Four, Suit.Clubs));
        pile.PlaceOnTop(new StandardPlayingCard(Rank.Four, Suit.Diamonds));
        table.Cards.Add(new KeyValuePair<DrawPile<StandardPlayingCard>, List<int>>(pile, [8]));
        table.AddCard(new StandardPlayingCard(Rank.Two, Suit.Clubs));
        Assert.True(table.BuildTable(0, 1, 10));
    }
}