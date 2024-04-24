namespace Peer;

public class MethodHandler
{
    public enum MethodType
    {
        PlaceCard,
        Build,
        BuildTable,
        Take,
        ClearTable
    }

    public static bool CallMethod(string methodString, IEnumerable<string> inputs)
    {
        var sanMethod = methodString.TrimStart('_');
        if (Enum.TryParse(sanMethod, true, out MethodHandler.MethodType method))
        {
            var input = inputs.Select(int.Parse).ToList();
            var table = Program.GameState.Table;
            var currentPlayer = Program.GameState.CurrentPlayer;
            var players = Program.GameState.Players;
            switch (method)
            {
                case MethodType.PlaceCard:
                    return input.Count == 1 && players[currentPlayer].PlaceCard(table, input[0]);
                case MethodType.Build:
                    return input.Count == 3 && players[currentPlayer].Build(table, input[0], input[1], input[2]);
                case MethodType.BuildTable:
                    return input.Count == 3 && players[currentPlayer].BuildTable(table, input[0], input[1], input[2]);
                case MethodType.Take:
                    return input.Count == 2 && players[currentPlayer].Take(table, input[0], input[1]);
                case MethodType.ClearTable:
                    return input.Count == 1 && players[currentPlayer].ClearTable(table, input[0]);
                default:
                    Console.WriteLine("Method not recognized.");
                    return false;
            }
        }

        Console.WriteLine("Invalid method string.");
        return false;
    }
}