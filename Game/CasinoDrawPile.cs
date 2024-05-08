using System.Text.Json.Serialization;
using Xyaneon.Games.Cards;
using Xyaneon.Games.Cards.StandardPlayingCards;

namespace Game;

public class CasinoDrawPile : DrawPile<StandardPlayingCard>
{
    public List<int> Values { get; set; }
    public Player? BelongTo { get; set; }

    public CasinoDrawPile(StandardPlayingCard? card = null, bool isFaceUp = false)
    {
        Cards = new Stack<StandardPlayingCard>();
        if (card != null)
        {
            PlaceOnTop(card);
        }

        IsFaceUp = isFaceUp;
    }
        
    [JsonConstructor]
    public CasinoDrawPile()
    {
    }

    public new void PlaceOnTop(StandardPlayingCard card)
    {
        base.PlaceOnTop(card);
        Values = GameState.CardToValue(card).Item1;
    }
}