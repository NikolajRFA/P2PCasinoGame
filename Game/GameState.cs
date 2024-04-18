using Xyaneon.Games.Cards.StandardPlayingCards;

namespace Game;

public class GameState
{
    public List<Player> Players { get; set; } = [];
    public Table Table { get; set; } = new();
    public int IndexOfNextPlayer { get; set; }
    public StandardPlayingCardDeck Deck = new();

    public GameState(List<string> players)
    {
        players.ForEach(player => Players.Add(new Player{Name = player}));
        Setup();
    }

    public static List<int> CardToValue(StandardPlayingCard card)
    {
        return card switch
        {
            { Suit: Suit.Diamonds, Rank: Rank.Ten } => [10, 16],
            { Suit: Suit.Spades, Rank: Rank.Two } => [2, 15],
            { Rank: Rank.Ace } => [1, 14],
            _ => [Convert.ToInt32(card.Rank)]
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
}