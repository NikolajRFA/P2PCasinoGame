namespace Peer;

public class MethodHandler
{
    public enum MethodType
    {
        PlaceCard,
        Build,
        Take,
        ClearTable
    }

    public static bool CallMethod(string methodString, IEnumerable<string> inputs)
    {
        var sanMethod = methodString.TrimStart('_');
        if (Enum.TryParse(sanMethod, true, out MethodType method))
        {
            List<int> input = [];
            List<int> tableIdxs = [];
            if (inputs.First().Contains('['))
            {
                var unsanitizedInput = inputs.First();
                tableIdxs = unsanitizedInput.Substring(1, unsanitizedInput.Length - 2).Split(",").Select(int.Parse).ToList();
            }
            else
            {
                input = inputs.Select(int.Parse).ToList();
            }
            var table = Program.GameState.Table;
            var currentPlayer = Program.GameState.CurrentPlayer;
            var players = Program.GameState.Players;
            switch (method)
            {
                case MethodType.PlaceCard:
                    return input.Count == 1 && players[currentPlayer].PlaceCard(table, input[0]);
                case MethodType.Build:
                    return input.Count == 3 && players[currentPlayer].Build(table, tableIdxs, input[0], input[1]);
                case MethodType.Take:
                    //return input.Count == 2 && players[currentPlayer].Take(table, tableIdxs, input[0]);
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