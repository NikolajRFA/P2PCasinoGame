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
        table.Cards.Add(new KeyValuePair<DrawPile<StandardPlayingCard>, List<int>>(drawPile,
            GameState.CardToValue(Hand[handIndex]).Item1));
        Hand.RemoveAt(handIndex);

        return true;
    }

    public bool Build(Table table, List<int> indexes, int handIndex, int value)
    {
        var filteredList = Hand.Where((_, index) => index != handIndex);
        if (!filteredList.Any(card => GameState.CardToValue(card).Item1.Contains(value))) return false;
        var kvps = table.Cards.Where((_, index) => indexes.Contains(index)).ToList();
        var cardValues = GameState.CardToValue(Hand[handIndex]).Item1;

        if (!CompareValues(value, kvps.Select(kvp => kvp.Value).Append(cardValues).ToList())) return false;

        foreach (var kvp in kvps.Where((_, idx) => idx != 0))
        {
            foreach (var card in kvp.Key.Cards)
            {
                kvps.First().Key.Cards.Push(card);
            }

            table.Cards.Remove(kvp);
        }

        kvps.First().Value.Clear();
        kvps.First().Value.Add(value);

        /*kvp.Key.PlaceOnTop(Hand[handIndex]);
        kvp.Value.Clear();
        kvp.Value.Add(value);*/
        return true;
    }

    public bool BuildTable(Table table, int index1, int index2, int value)
    {
        var cards1 = table.Cards[index1].Value;
        var cards2 = table.Cards[index2].Value;
        //return CompareValues(cards1, cards2, value);
        return false;
    }

    public bool Take(Table table, int index, int handIndex)
    {
        if (table.Cards[index].Value.Any(x => GameState.CardToValue(Hand[handIndex]).Item1.Any(y => x == y)))
        {
            foreach (var tableCard in table.Cards[index].Key.Cards)
            {
                PointPile.Add(tableCard);
                PointPile.Add(Hand[handIndex]);
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

    private bool CompareValues(int value, List<List<int>> valuesCollections, int index = 0, int currentSum = 0)
    {
        // Base case: If we've checked all collections and the current sum equals the target value, return true.
        if (index == valuesCollections.Count)
        {
            return currentSum == value;
        }

        // Recursive case: Try each value in the current collection.
        return valuesCollections.ElementAt(index)
            .Any(item => 
                CompareValues(value, valuesCollections, index + 1, currentSum + item));
    }
}