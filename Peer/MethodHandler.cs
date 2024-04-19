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
            List<int> parsedInputs = inputs.Select(int.Parse).ToList();
            switch (method)
            {
                case MethodType.PlaceCard:
                    Program.GameState.Players[Program.GameState.CurrentPlayer]
                        .PlaceCard(Program.GameState.Table, parsedInputs[0]);
                    Console.WriteLine($"PlaceCard method ran with input {inputs[0]}");
                    break;
                case MethodType.Build:
                    if (Program.GameState.Players[Program.GameState.CurrentPlayer]
                        .Build(Program.GameState.Table, parsedInputs[0], parsedInputs[1], parsedInputs[2]))
                    {
                        Console.WriteLine("The method returned true");
                    }
                    else
                    {
                        Console.WriteLine("The method returned false");
                    }

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