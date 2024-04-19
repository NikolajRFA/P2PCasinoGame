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

    public static void CallMethod(string methodString, List<string> inputs)
    {
        if (Enum.TryParse(methodString, out MethodHandler.MethodType method))
        {
            switch (method)
            {
                case MethodType.PlaceCard:
                    Program.GameState.Players[Program.GameState.CurrentPlayer]
                        .PlaceCard(Program.GameState.Table, int.Parse(inputs[0]));
                    Console.WriteLine($"PlaceCard method ran with input {inputs[0]}");
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