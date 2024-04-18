using Xyaneon.Games.Cards;
using Xyaneon.Games.Cards.StandardPlayingCards;

namespace Game;

public class Player
{
    public string Name { get; set; }
    public List<StandardPlayingCard> Hand { get; set; } = [];
    public List<StandardPlayingCard> PointPile { get; set; } = [];
    public int ClearCount { get; set; }

    public bool PlaceCard(Table table, int handIndex)
    {
        var drawPile = new DrawPile<StandardPlayingCard>(isFaceUp: true);
        drawPile.PlaceOnTop(Hand[handIndex]);
        table.Cards.Add(new KeyValuePair<DrawPile<StandardPlayingCard>, List<int>>(drawPile, GameState.CardToValue(Hand[handIndex]).Item1));
        Hand.RemoveAt(handIndex);
        
        return true;
    }

    public bool Build(Table table, int index, int handIndex, int value)
    {
        var kvp = table.Cards[index];
        var cardValues = GameState.CardToValue(Hand[handIndex]).Item1;

        if (!CompareValues(kvp.Value, cardValues, value)) return false;

        kvp.Key.PlaceOnTop(Hand[handIndex]);
        kvp.Value.Clear();
        kvp.Value.Add(value);
        return true;
    }

    public bool BuildTable(Table table, int index1, int index2, int value)
    {
        var cards1 = table.Cards[index1].Value;
        var cards2 = table.Cards[index2].Value;
        return CompareValues(cards1, cards2, value);
    }

    public bool Take(Table table, int index, int handIndex)
    {
        if (table.Cards[index].Value.Any(x => GameState.CardToValue(Hand[handIndex]).Item1.Any(y => x == y)))
        {
            foreach (var tableCard in table.Cards[index].Key.Cards)
            {
                PointPile.Add(tableCard);
            }

            if (table.Cards.Count == 1) ClearCount++;
            Hand.RemoveAt(handIndex);
            table.Cards.RemoveAt(index);
            return true;
        }

        return false;
    }

    public bool ClearTable(Table table, int handIndex)
    {
        if (Hand[handIndex] != new StandardPlayingCard(Rank.Five, Suit.Spades)) return false;
        foreach (var card in table.Cards)
        {
            PointPile.AddRange(card.Key.Cards);
        }
        Hand.RemoveAt(handIndex);
        table.Cards.Clear();
        return true;
    }
    
    private bool CompareValues(IEnumerable<int> values1, IEnumerable<int> values2, int value)
    {
        return values1.Any(cardValue => values2.Any(tableValue => cardValue + tableValue == value));
    }
}