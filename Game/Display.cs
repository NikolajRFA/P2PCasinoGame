using System.Reflection.Metadata.Ecma335;
using System.Text;
using Game;
using Xyaneon.Games.Cards;

namespace Game;

public class Display(GameState gameState, string playerName)
{
    public GameState GameState { get; set; } = gameState;
    public string PlayerName { get; set; } = playerName;

    public string Hand()
    {
        return
            $"Hand: {string.Join(" | ", GameState.Players.Single(player => player.Name == PlayerName).Hand.Select(card => card.ToString()))}";
    }

    public string Table()
    {
        return
            $"Table: {string.Join(" | ", GameState.Table.Cards.Select(pile => string.Join(", ", pile.Key.Cards.Select(card => card.ToString())) + (pile.Key.Cards.Count > 1 ? $" ({pile.Value.Single()})" : "")))}";
    }

    public override string ToString()
    {
        return $"{Hand()}\n{Table()}";
    }
}