using Xyaneon.Games.Cards;
using Xyaneon.Games.Cards.StandardPlayingCards;

namespace Game;

public class GameState
{
    public List<Player> Players { get; set; }
    public Table Table { get; set; }
    public int IndexOfNextPlayer { get; set; }
    public StandardPlayingCardDeck Deck = new();

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
    }

    public void Deal()
    {
        //    for
    }
}