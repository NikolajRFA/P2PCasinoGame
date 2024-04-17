using Game;
using Xyaneon.Games.Cards.StandardPlayingCards;

namespace TestProject1;

public class PlayerTests
{
    [Fact]
    public void Take_Table5WithHand5_True()
    {
        var table = new Table();
        table.AddCard(new StandardPlayingCard(Rank.Five, Suit.Hearts));
        var player = new Player { Name = "Alex" };
        
        Assert.True(player.Take(table, 0, new StandardPlayingCard(Rank.Five, Suit.Diamonds)));
    }

    [Fact]
    public void Take_Table5WithHand6_False()
    {
        var table = new Table();
        table.AddCard(new StandardPlayingCard(Rank.Five, Suit.Hearts));
        var player = new Player { Name = "Alex" };
        
        Assert.False(player.Take(table, 0, new StandardPlayingCard(Rank.Six, Suit.Diamonds)));
    }

    [Fact]
    public void ClearTable_PlayerPlays5OfSpades_True()
    {
        var table = new Table();
        table.AddCard(new StandardPlayingCard(Rank.Five, Suit.Clubs));
        var player = new Player();
        
        Assert.True(player.ClearTable(table, new StandardPlayingCard(Rank.Five, Suit.Spades)));
    }
}