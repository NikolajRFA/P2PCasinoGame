using Xyaneon.Games.Cards;
using Xyaneon.Games.Cards.StandardPlayingCards;

namespace Game;

public class Table
{
    public List<KeyValuePair<DrawPile<StandardPlayingCard>, List<int>>> Cards { get; set; } = [];

    public bool AddCard(StandardPlayingCard card)
    {
        var drawPile = new DrawPile<StandardPlayingCard>(isFaceUp: true);
        drawPile.PlaceOnTop(card);
        Cards.Add(new KeyValuePair<DrawPile<StandardPlayingCard>, List<int>>(drawPile, GameState.CardToValue(card)));
        return true;
    }

    public bool Build(int index, StandardPlayingCard card, int value)
    {
        var kvp = Cards[index];
        var cardValues = GameState.CardToValue(card);

        if (!CompareValues(kvp.Value, cardValues, value)) return false;

        kvp.Key.PlaceOnTop(card);
        kvp.Value.Clear();
        kvp.Value.Add(value);
        return true;
    }

    public bool BuildTable(int index1, int index2, int value)
    {
        var cards1 = Cards[index1].Value;
        var cards2 = Cards[index2].Value;
        return CompareValues(cards1, cards2, value);
    }

    private bool CompareValues(IEnumerable<int> values1, IEnumerable<int> values2, int value)
    {
        return values1.Any(cardValue => values2.Any(tableValue => cardValue + tableValue == value));
    }
}