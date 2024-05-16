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
        // Check if any piles belong to me
        if (table.Piles.Any(pile => pile.BelongTo == this)) return false;
        var drawPile = new DrawPile<StandardPlayingCard>(isFaceUp: true);
        drawPile.PlaceOnTop(Hand[handIndex]);
        table.Piles.Add(new CasinoDrawPile(card: Hand[handIndex]));
        //table.Piles.Add(new Table.ValuePile(drawPile,
        //    GameState.CardToValue(Hand[handIndex]).Item1));
        Hand.RemoveAt(handIndex);

        return true;
    }

    public bool Build(Table table, List<int> indexes, int handIndex, int value)
    {
        var filteredList = Hand.Where((_, index) => index != handIndex);
        if (!filteredList.Any(card => GameState.CardToValue(card).Item1.Contains(value))) return false;
        var valuePiles = table.Piles.Where((_, index) => indexes.Contains(index)).ToList();
        var cardValues = GameState.CardToValue(Hand[handIndex]).Item1;

        
        if (!GenerateCombinations(valuePiles.Select(valuePile => valuePile.Values).Append(cardValues).ToList())
            .Any(list => CanSumToTarget(list, value))) return false;

        foreach (var valuePile in valuePiles.Where((_, idx) => idx != 0))
        {
            foreach (var card in valuePile.Cards)
            {
                valuePiles.First().Cards.Push(card);
            }

            table.Piles.Remove(valuePile);
        }

        valuePiles.First().Cards.Push(Hand[handIndex]);
        Hand.RemoveAt(handIndex);
        valuePiles.First().Values = [value];
        valuePiles.First().BelongTo = this;

        return true;
    }

    public bool Take(Table table, List<int> indexes, int handIndex)
    {
        var tableCards = table.Piles.Where((_, index) => indexes.Contains(index));
        // Get all value combinaions from cards on the table.
        var listCombinations = GenerateCombinations(tableCards.Select(kvp => kvp.Values).ToList());
        // Using stringbuilder to describe what was taken
        StringBuilder description = new();
        var cardInHand = Hand[handIndex];
        
        if (listCombinations.Any(list => GameState.CardToValue(cardInHand).Item1.Any(val => CanSumToTarget(list, val))))
        {
            foreach (var pile in tableCards)
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

            if (table.Piles.Count == 1) ClearCount++;

            indexes.Sort();
            indexes.Reverse();
            indexes.ForEach(idx => table.Piles.RemoveAt(idx));
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
        foreach (var card in table.Piles)
        {
            PointPile.AddRange(card.Cards);
        }
        PointPile.Add(Hand[handIndex]);
        ClearCount++;
        Hand.RemoveAt(handIndex);
        table.Piles.Clear();
        return true;
    }

    private static bool CanSumToTarget(List<int> numbers, int target)
        {
            if (numbers.Any(number => number > target)) return false;
            // If the numbers sum to the target, we can return true
            if (numbers.Sum() == target) return true;
            // Calculate the total sum of the list
            int totalSum = numbers.Sum();

            // Check if the total sum is divisible by the target
            // If not, it's impossible to form sublists that sum up to the target
            if (totalSum % target != 0) return false;

            // Calculate the number of sublists needed
            int sublistsNeeded = totalSum / target;

            // Check if the number of sublists needed matches the number of elements in the list
            if (sublistsNeeded > numbers.Count) return false;

            // Sort the list in descending order to ensure the largest numbers are considered first
            numbers.Sort();

            // Recursively check if a subset can sum up to the target
            return CanSumToTargetRecursive(numbers, target, new HashSet<int>());
        }

        private static bool CanSumToTargetRecursive(List<int> numbers, int target, HashSet<int> used)
        {
            if (target == 0) return true; // Base case: target reached
            if (numbers.Count == 0) return false; // Base case: no numbers left

            foreach (var currentNumber in numbers)
            {
                if (used.Add(currentNumber))
                {
                    var number = currentNumber;
                    if (CanSumToTargetRecursive(numbers.Where(x => x != number).ToList(), target - currentNumber, used))
                    {
                        return true;
                    }

                    used.Remove(currentNumber); // Backtrack
                }
            }

            return false;
        }

        private static List<List<int>> GenerateCombinations(List<List<int>> input)
        {
            // Base case: if the input list is empty, return an empty list.
            if (input.Count == 0)
            {
                return new List<List<int>>();
            }

            var result = new List<List<int>>();
            var firstList = input.First();
            var remainingLists = input.Skip(1).ToList();

            foreach (var number in firstList)
            {
                var combinations = GenerateCombinations(remainingLists);

                // If there are no combinations from the remaining lists,
                // just add the current number as a single-element list.
                if (combinations.Count == 0)
                {
                    result.Add(new List<int> { number });
                }
                else
                {
                    // Combine the current number with each combination.
                    foreach (var combination in combinations)
                    {
                        combination.Add(number);
                        result.Add(combination);
                    }
                }
            }

            return result;
        }
}