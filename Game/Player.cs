using System.Text;
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

        kvps.First().Key.Cards.Push(Hand[handIndex]);
        Hand.RemoveAt(handIndex);
        kvps.First().Value.Clear();
        kvps.First().Value.Add(value);

        return true;
    }

    public bool Take(Table table, List<int> indexes, int handIndex)
    {
        var tableCards = table.Cards.Where((_, index) => indexes.Contains(index));
        // Get possible sums when combining card values.
        var possibleSums = GetPossibleSums(tableCards.Select(kvp => kvp.Value).ToList());
        // Using stringbuilder to describe what was taken
        StringBuilder description = new();
        var cardInHand = Hand[handIndex];

        if (possibleSums.Any(sum =>
                GameState.CardToValue(cardInHand).Item1.Any(val => val == sum || sum % val == 0)))
        {
            foreach (var pile in tableCards.Select(kvp => kvp.Key))
            {
                var count = pile.Cards.Count;
                for (var i = 0; i < count; i++)
                {
                    var card = pile.Cards.Pop();
                    description.Append($"{card.Rank} of {card.Suit}\n");
                    PointPile.Add(card);
                }
            }

            description.Append($"Was taken with: {cardInHand.Rank} of {cardInHand.Suit}");
            PointPile.Add(Hand[handIndex]);

            if (table.Cards.Count == 1) ClearCount++;

            indexes.Sort();
            indexes.Reverse();
            indexes.ForEach(idx => table.Cards.RemoveAt(idx));
            Hand.RemoveAt(handIndex);
            Console.WriteLine(description);
            return true;
        }

        return false;
    }

    public bool ClearTable(Table table, int handIndex)
    {
        if (handIndex == -1) return false;
        if (Hand[handIndex] != new StandardPlayingCard(Rank.Five, Suit.Spades)) return false;
        foreach (var card in table.Cards)
        {
            PointPile.AddRange(card.Key.Cards);
        }

        Hand.RemoveAt(handIndex);
        table.Cards.Clear();
        return true;
    }

    private bool CompareValues(int value, List<List<int>> valuesCollections)
    {
        var possibleSums = GetPossibleSums(valuesCollections);
        return possibleSums.Contains(value) || possibleSums.Any(sum => sum % value == 0);
    }

    private static List<int> GetPossibleSums(List<List<int>> listOfLists)
    {
        List<int> possibleSums = new List<int>();
        GetSumsRecursively(listOfLists, 0, 0, possibleSums);
        return possibleSums;
    }

    private static void GetSumsRecursively(List<List<int>> listOfLists, int index, int currentSum,
        List<int> possibleSums)
    {
        if (index == listOfLists.Count)
        {
            possibleSums.Add(currentSum);
            return;
        }

        foreach (var value in listOfLists[index])
        {
            GetSumsRecursively(listOfLists, index + 1, currentSum + value, possibleSums);
        }
    }
}