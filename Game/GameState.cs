using Xyaneon.Games.Cards.StandardPlayingCards;

namespace Game;

public class GameState
{
    public List<Player> Players { get; set; } = [];
    public Table Table { get; set; } = new();
    public int CurrentPlayer { get; set; }
    public StandardPlayingCardDeck Deck = new();

    public GameState(List<string> players)
    {
        players.ForEach(player => Players.Add(new Player { Name = player }));
        Setup();
    }

    /// <summary>
    /// Method used to convert a card to it`s value and points for special cards.
    /// </summary>
    /// <param name="card">An arbitrary standard playing card</param>
    /// <returns>A tuple containing the card value and the amount of points.</returns>
    public static (List<int>, int) CardToValue(StandardPlayingCard card)
    {
        return card switch
        {
            { Suit: Suit.Diamonds, Rank: Rank.Ten } => ([10, 16], 2),
            { Suit: Suit.Spades, Rank: Rank.Two } => ([2, 15], 1),
            { Rank: Rank.Ace } => ([1, 14], 1),
            _ => ([Convert.ToInt32(card.Rank)], 0)
        };
    }

    public void Setup()
    {
        Deck.Shuffle();
        Deal();
        var fourCards = Deck.DrawAtMost(4);
        Table.AddCards(fourCards);
    }

    public void Deal()
    {
        Players.ForEach(player =>
        {
            var fourCards = Deck.DrawAtMost(4);
            player.Hand.AddRange(fourCards);
        });
    }

    public List<(Player, int)> SumPoints()
    {
        List<(Player, int)> output = [];
        var minClears = Players.Select(x => x.ClearCount).Min();
        var playerWithMaxCards = Players.MaxBy(player => player.PointPile.Count);
        var playerWithMaxSpades = Players.MaxBy(player => player.PointPile.Count(card => card.Suit == Suit.Spades));
        Players.ForEach(player =>
        {
            player.ClearCount -= minClears;
            var points = player.PointPile.Sum(card => CardToValue(card).Item2) + player.ClearCount;
            if (playerWithMaxCards == player) points += 1;
            if (playerWithMaxSpades == player) points += 2;
            output.Add((player, points));
        });
        return output;
    }

    public void AdvanceTurn()
    {
        if (CurrentPlayer < Players.Count - 1) CurrentPlayer++;
        else CurrentPlayer = 0;
    }
}