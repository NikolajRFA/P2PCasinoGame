using Game;
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
}