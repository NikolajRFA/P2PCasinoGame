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

        if (!cardValues.Any(cardValue => kvp.Value.Any(tableValue => cardValue + tableValue == value))) return false;

        kvp.Key.PlaceOnTop(card);
        return true;
    }
}