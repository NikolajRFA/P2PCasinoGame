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

    public static void CallMethod(string methodString, List<int> inputs)
    {
        if (Enum.TryParse(methodString, out MethodHandler.MethodType method))
        {
            switch (method)
            {
                case MethodType.PlaceCard:
                    Program.GameState.Players[Program.GameState.CurrentPlayer]
                        .PlaceCard(Program.GameState.Table, inputs[0]);
                    Console.WriteLine($"PlaceCard method ran with input {inputs[0]}");
                    break;
                case MethodType.Build:
                    Program.GameState.Players[Program.GameState.CurrentPlayer]
                        .Build(Program.GameState.Table, inputs[0], inputs[1], inputs[2]);
                    Console.WriteLine($"Build method ran with input {inputs[0]}, {inputs[1]}, {inputs[2]}");
                    break;
                // Add cases for other methods as needed
                default:
                    Console.WriteLine("Method not recognized.");
                    break;
            }
        }
        else
        {
            Console.WriteLine("Invalid method string.");
        }
    }

}