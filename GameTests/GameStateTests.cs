using System.Text.Json;
using Game;
using Xunit.Abstractions;
using Xyaneon.Games.Cards.StandardPlayingCards;

namespace TestProject1;

public class GameStateTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public GameStateTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Constructor_3Players_3PlayersReceive4CardsEachAndTableHas4Cards()
    {
        var gameState = new GameState(["Alex", "Nikolaj", "Laust"]);
        gameState.Players.ForEach(player =>
        {
            _testOutputHelper.WriteLine(
                $"Player: {player.Name} has cards {string.Join(", ", player.Hand.Select(card => $"{card.Rank} of {card.Suit}"))}");
            Assert.True(player.Hand.Count == 4);
        });
        gameState.Table.Cards.ForEach(kvp => Assert.NotEmpty(kvp.Key.Cards));
    }

    [Fact]
    public void SumPoints_AllCardsToOnePlayer_11()
    {
        var gameState = new GameState(["Alex"]);
        var table = new Table();
        var player = gameState.Players[0];
        // setting up table with a fiver
        player.Hand.Add(new StandardPlayingCard(Rank.Five, Suit.Spades));
        player.PlaceCard(table, 0);
        // taking the card on the table with card in hand
        player.Hand.Add(new StandardPlayingCard(Rank.Five, Suit.Diamonds));
        player.Take(table, [0], 0);
        // Hacking an advancement of the turn to make the current player the last to take
        gameState.AdvanceTurn("_take");
        // Adding a full set of cards to player's hand
        player.PointPile.AddRange(new StandardPlayingCardDeck().Cards);
        Assert.Equal(11, gameState.SumPoints()[0].Item2);
    }

    [Fact]
    public void SerializeDeserialize_GameState_EqualsAfterDeserialization()
    {
        var gameState = new GameState(["Alex", "Nikolaj"]);
        var json = gameState.Serialize();
        var gameStateDeserialized = GameState.Deserialize(json);
        Assert.Equal(gameState.Serialize(), gameStateDeserialized.Serialize());
    }

    [Fact]
    public void Reverse_StandardDeck_Success()
    {
        var deck1 = new StandardPlayingCardDeck();
        var deck2 = new StandardPlayingCardDeck();
        deck2.Reverse();
        Assert.NotEqual(deck1.Draw(), deck2.Draw());
    }

    [Fact]
    public void NewGameState_Display_Rank()
    {
        var gameState = new GameState(["Alex", "Nikolaj", "Laust"]);
        var display = gameState.DisplayHand("Alex");
        _testOutputHelper.WriteLine(display);
    }
    
    [Fact]
    public void CanSumToTarget_GenerateCombinationsFromListListInt_Correct()
    {
        Assert.Contains(GenerateCombinations(
                [[5], [5], [10, 16], [7], [2], [1, 14], [8], [2, 15]]),
            list => CanSumToTarget(list, 10)
        );
        Assert.DoesNotContain(
            GenerateCombinations([[5], [5], [10, 16], [7], [2], [1, 14], [8], [2, 15], [1]]),
            list => CanSumToTarget(list, 10)
        );

        bool CanSumToTarget(List<int> numbers, int target)
        {
            if (numbers.Any(number => number > target)) return false;
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

        bool CanSumToTargetRecursive(List<int> numbers, int target, HashSet<int> used)
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

        List<List<int>> GenerateCombinations(List<List<int>> input)
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
                if (!combinations.Any())
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
}