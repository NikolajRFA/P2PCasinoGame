using Xyaneon.Games.Cards.StandardPlayingCards;

namespace Game;

public class Player
{
    public string Name { get; set; }
    public List<StandardPlayingCard> Hand { get; set; } = [];
    public Dictionary<StandardPlayingCard, bool> PointPile { get; set; } = [];

    public bool Take(Table table, int index, StandardPlayingCard card)
    {
        if (table.Cards[index].Value.Any(x => GameState.CardToValue(card).Any(y => x == y)))
        {
            foreach (var tableCard in table.Cards[index].Key.Cards)
            {
                PointPile.Add(tableCard, table.Cards.Count == 1);
            }

            return true;
        }

        return false;
    }
}