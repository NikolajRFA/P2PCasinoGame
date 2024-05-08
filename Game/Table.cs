using Xyaneon.Games.Cards;
using Xyaneon.Games.Cards.StandardPlayingCards;

namespace Game;

public class Table
{
    public List<CasinoDrawPile> Piles { get; set; } = [];

    public void AddCards(IEnumerable<StandardPlayingCard> cards)
    {
        foreach (var card in cards)
        {
            var pile = new CasinoDrawPile(isFaceUp: true);
            pile.PlaceOnTop(card);

            Piles.Add(pile);
        }
    }

    public class ValuePile(DrawPile<StandardPlayingCard> pile, List<int> values)
    {
        public DrawPile<StandardPlayingCard> Pile { get; set; } = pile;
        public List<int> Values { get; set; } = values;
        public Player? BelongTo { get; set; }
    }
}