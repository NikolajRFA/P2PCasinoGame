using Xyaneon.Games.Cards;
using Xyaneon.Games.Cards.StandardPlayingCards;

namespace Game;

public class Table
{
    public List<ValuePile> Piles { get; set; } = [];

    public void AddCards(IEnumerable<StandardPlayingCard> cards)
    {
        foreach (var card in cards)
        {
            var drawPile = new DrawPile<StandardPlayingCard>(isFaceUp: true);
            drawPile.PlaceOnTop(card);
            Piles.Add(new ValuePile(drawPile, GameState.CardToValue(card).Item1));
        }
    }

    public class ValuePile(DrawPile<StandardPlayingCard> pile, List<int> values)
    {
        public DrawPile<StandardPlayingCard> Pile { get; set; } = pile;
        public List<int> Values { get; set; } = values;
    }
}