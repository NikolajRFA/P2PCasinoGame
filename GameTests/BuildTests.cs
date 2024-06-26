﻿using Game;
using Xyaneon.Games.Cards.StandardPlayingCards;

namespace TestProject1;

public class BuildTests
{
    [Fact]
    public void Build_SingleValuePlusSingleValue_True()
    {
        var table = new Table();
        var player = new Player();
        player.Hand.Add(new StandardPlayingCard(Rank.Two, Suit.Clubs));
        player.PlaceCard(table, 0);
        player.Hand.Add(new StandardPlayingCard(Rank.Three, Suit.Clubs));
        player.Hand.Add(new StandardPlayingCard(Rank.Five, Suit.Diamonds));
        Assert.True(player.Build(table, [0], 0, 5));
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
        Assert.True(player.Build(table, [0], 0, 4));
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
        Assert.True(player.Build(table, [0], 0, 4));
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
        Assert.True(player.Build(table, [0], 0, 2));
    }

    [Fact]
    public void Build_5Plus5Is12_False()
    {
        var table = new Table();
        var player = new Player();
        player.Hand.Add(new StandardPlayingCard(Rank.Five, Suit.Diamonds));
        player.PlaceCard(table, 0);
        player.Hand.Add(new StandardPlayingCard(Rank.Five, Suit.Clubs));
        Assert.False(player.Build(table, [0], 0, 12));
    }

    [Fact]
    public void Build_AcePlus5Plus10Is16_True()
    {
        var table = new Table();
        var player = new Player();
        // Hack together table setup
        player.Hand.AddRange([
            new StandardPlayingCard(Rank.Ace, Suit.Diamonds), new StandardPlayingCard(Rank.Five, Suit.Clubs)
        ]);
        player.PlaceCard(table, 0);
        player.PlaceCard(table, 0);
        // Setup player
        player.Hand.AddRange([
            new StandardPlayingCard(Rank.Ten, Suit.Spades), new StandardPlayingCard(Rank.Ten, Suit.Diamonds)
        ]);
        Assert.True(player.Build(table, [0, 1], 0, 16));
    }

    [Fact]
    public void Build_TableAcePlusTable10PlusHand11Is11_True()
    {
        var table = new Table();
        var player = new Player();
        // Hack together table setup
        player.Hand.AddRange([
            new StandardPlayingCard(Rank.Ace, Suit.Diamonds), new StandardPlayingCard(Rank.Ten, Suit.Clubs)
        ]);
        player.PlaceCard(table, 0);
        player.PlaceCard(table, 0);
        // Setup player
        player.Hand.AddRange([
            new StandardPlayingCard(Rank.Jack, Suit.Clubs), new StandardPlayingCard(Rank.Jack, Suit.Diamonds)
        ]);
        Assert.True(player.Build(table, [0, 1, 2], 0, 11));
    }

    [Fact]
    public void Build_FivePlusFivePlusFive_BelongsToMe()
    {
        var table = new Table();
        var player = new Player();
        // Hack together table setup
        player.Hand.AddRange([
            new StandardPlayingCard(Rank.Five, Suit.Clubs), new StandardPlayingCard(Rank.Five, Suit.Diamonds)
        ]);
        player.PlaceCard(table, 0);
        player.PlaceCard(table, 0);
        // Setup player
        player.Hand.AddRange([
            new StandardPlayingCard(Rank.Five, Suit.Hearts), new StandardPlayingCard(Rank.Five, Suit.Spades)
        ]);
        Assert.True(player.Build(table, [0, 1], 0, 5));
        Assert.Contains(table.Piles, pile => pile.BelongTo == player);
        Assert.Single(table.Piles);
    }

    [Fact]
    public void Build_FivePlusFivePlusFive_CannotPlaceCardWhileBuildingBelongsToMe()
    {
        var table = new Table();
        var player = new Player();
        // Hack together table setup
        player.Hand.AddRange([
            new StandardPlayingCard(Rank.Five, Suit.Clubs), new StandardPlayingCard(Rank.Five, Suit.Diamonds)
        ]);
        player.PlaceCard(table, 0);
        player.PlaceCard(table, 0);
        // Setup player
        player.Hand.AddRange([
            new StandardPlayingCard(Rank.Five, Suit.Hearts), new StandardPlayingCard(Rank.Five, Suit.Spades)
        ]);
        Assert.True(player.Build(table, [0, 1], 0, 5));
        // Add another card to the players hand for place
        Assert.False(player.PlaceCard(table, 0));
    }

    [Fact]
    public void Build_TwoPlusEightAnotherPlayerBuildsOnTop_FirstPlayerCanPlaceCard()
    {
        var table = new Table();
        var player1 = new Player();
        var player2 = new Player();
        // Hack together table setup.
        player1.Hand.AddRange([
            new StandardPlayingCard(Rank.Two, Suit.Diamonds),
            new StandardPlayingCard(Rank.Eight, Suit.Clubs),
            new StandardPlayingCard(Rank.Ten, Suit.Hearts)
        ]);
        player1.PlaceCard(table, 0);
        Assert.True(player1.Build(table, [0], 0, 10));
        player2.Hand.AddRange([
            new StandardPlayingCard(Rank.Two, Suit.Hearts), 
            new StandardPlayingCard(Rank.Queen, Suit.Spades)
        ]);
        Assert.True(player2.Build(table, [0], 0, 12));
        Assert.True(player1.PlaceCard(table, 0));
        Assert.False(player2.PlaceCard(table, 0));
    }
}