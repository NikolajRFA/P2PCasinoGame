using Xyaneon.Games.Cards;
using Xyaneon.Games.Cards.StandardPlayingCards;

namespace Game;

public class Table
{
    public List<KeyValuePair<DrawPile<StandardPlayingCard>, List<int>>> Cards { get; set; } = [];

    public void AddCard(StandardPlayingCard card)
    {
        Cards.Add(new KeyValuePair<DrawPile<StandardPlayingCard>, List<int>>(
            new DrawPile<StandardPlayingCard>(isFaceUp: true), GameState.CardToValue(card)
        ));
    }
}