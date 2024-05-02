using System.Text.Json;
using System.Text.Json.Serialization;
using Xyaneon.Games.Cards;
using Xyaneon.Games.Cards.StandardPlayingCards;

namespace Game;

public class GameState
{
    public List<Player> Players { get; init; } = [];
    public Table Table { get; init; } = new();
    public int CurrentPlayer { get; set; }
    public StandardPlayingCardDeck Deck { get; init; } = new StandardPlayingCardDeck();
    public Player LastToTake { get; set; }

    // Deserialization constructor
    [JsonConstructor]
    public GameState(List<Player> players, Table table, int currentPlayer, StandardPlayingCardDeck deck)
    {
        Players = players;
        Table = table;
        CurrentPlayer = currentPlayer;
        Deck = deck;
    }

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
        Console.WriteLine("Cards dealt!");
    }

    // TODO: Make points more transparent. I.e. print out where the points come from and how many clear count each player has.
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
            if (LastToTake == player)
            {
                if (Table.Cards.Count > 0)
                {
                    foreach (var kvp in Table.Cards)
                    {
                        foreach (var card in kvp.Key.Cards)
                        {
                            player.PointPile.Add(card);
                        }
                    }
                }

                points += 1;
            }

            if (playerWithMaxCards == player) points += 1;
            if (playerWithMaxSpades == player) points += 2;
            output.Add((player, points));
        });
        return output;
    }

    public void AdvanceTurn(string method)
    {
        if (method.StartsWith("_take")) LastToTake = Players[CurrentPlayer];
        if (CurrentPlayer < Players.Count - 1) CurrentPlayer++;
        else CurrentPlayer = 0;

        if (Players.All(x => x.Hand.Count == 0))
        {
            if (Deck.IsEmpty)
            {
                var results = SumPoints();
                results.ForEach(result => Console.WriteLine($"{result.Item1.Name} has {result.Item2} points\n"));
            }
            else
            {
                Deal();
            }
        }
    }

    public string DisplayHand(string playerName)
    {
        return
            $"Hand: {string.Join(" | ", Players.Single(player => player.Name == playerName).Hand.Select(card => card.ToString()))}";
    }

    public string DisplayTable()
    {
        return
            $"Table: {string.Join(" | ", Table.Cards.Select(pile => string.Join(", ", pile.Key.Cards.Select(card => card.ToString())) + (pile.Key.Cards.Count > 1 ? $" ({pile.Value.Single()})" : "")))}";
    }

    public string DisplayGame(string player)
    {
        return
            $"\n{DisplayTable()}\n" +
            $"~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" +
            $"\n{DisplayHand(player)}\n";
    }

    public string Serialize()
    {
        return JsonSerializer.Serialize(this);
    }

    public static GameState Deserialize(string json)
    {
        var gameStateDeserialized =
            JsonSerializer.Deserialize<GameState>(json);
        gameStateDeserialized!.Deck.Reverse();
        return gameStateDeserialized;
    }
}