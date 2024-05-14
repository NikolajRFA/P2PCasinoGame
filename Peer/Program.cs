// See https://aka.ms/new-console-template for more information

using System.Security.Cryptography;
using System.Text;
using Game;
using Peer;
using Sharprompt;

namespace Peer;
// Implement security

public class Program
{
    public const int Port = 8000;

    //public static int GameState { get; set; }
    public static string MyIp = "172.29.0.11";
    public static GameState GameState { get; set; }
    public static RSA RSA { get; set; } = RSA.Create();
    public static Aes Aes { get; set; } = Aes.Create();

    public static void Main(string[] args)
    {
        SetupReceiver();

        SetupLobby();

        AwaitGameStart();

        HandleTurn();
    }

    /// <summary>
    /// Prompts the user if they would like to create or join a lobby.
    /// If create/host is selected <see cref="CreateLobby"/> is called.
    /// Else <see cref="JoinLobby"/> is called
    /// </summary>
    private static void SetupLobby()
    {
        while (true)
        {
            //Console.WriteLine("Do you want to >await< or create a connection >manually<?");
            var command = Prompt.Select("Do you want to host or join?", ["host", "join"]);
            if (command.Equals("host"))
            {
                if (CreateLobby()) break;
            }

            JoinLobby();
            break;
        }
    }

    /// <summary>
    /// <para>Method used to handle the flow of turns between players.</para>
    /// If it's the player's turn we prompt the user for an action by calling <see cref="MakeMove"/>
    /// with the cards on the table and the derived available actions.
    /// When the action has been generated we communicate the action to other peers.
    /// <para>Else we wait until it's the players turn yet again.</para>
    /// </summary>
    private static void HandleTurn()
    {
        while (true)
        {
            if (GameState.Players[GameState.CurrentPlayer].Name == MyIp)
            {
                var tableCards = ExtractTableCardsToList();
                var actions = FilterPlayerActions(tableCards, out var handCards);

                var message = MakeMove(actions, handCards, tableCards);

                Console.WriteLine(message);
                Outbound.Broadcast(message);
            }
            else
            {
                Console.WriteLine("Wait for your turn...");
                while (true)
                {
                    if (GameState.Players[GameState.CurrentPlayer].Name == MyIp) break;
                    Thread.Sleep(500);
                }
            }
        }
    }

    /// <summary>
    /// Method handling the player's move.
    /// </summary>
    /// <param name="actions">A filtered array of possible actions depending on the cards in the player's hand</param>
    /// <param name="handCards">The cards on the player's hand</param>
    /// <param name="tableCards">The cards on the table</param>
    /// <returns>A message string representing the player's move</returns>
    private static string MakeMove(string[] actions, List<string> handCards, List<string> tableCards)
    {
        var input = Prompt.Select("Make your move", actions);
        if (input == "QUIT")
        {
            if (!QuitGame()) input = Prompt.Select("Make your move", actions);
        }

        var method = "";
        var parameters = new StringBuilder();
        List<int> idxs = [];
        switch (input)
        {
            case "Place a card":
                method = "_placecard";
                var card = Prompt.Select("What card do you want to place?", handCards);
                parameters.Append(CH.BuildParameters(handCards.IndexOf(card)));
                break;
            case "Build":
                method = "_build";
                var buildCards = Prompt.MultiSelect("Where do you want to build on the table?", tableCards);
                idxs.AddRange(buildCards.Select(card => tableCards.IndexOf(card)));
                var handCard = Prompt.Select("Which card on your hand do you want to build with?", handCards);
                var value = Prompt.Input<int>("What is the value of the building?");
                parameters.Append(
                    CH.BuildParameters(idxs, handCards.IndexOf(handCard), value));

                break;
            case "Take":
                method = "_take";
                var takeCards = Prompt.MultiSelect("What do you want to take on the table?", tableCards);
                idxs.AddRange(takeCards.Select(card => tableCards.IndexOf(card)));
                handCard = Prompt.Select("Which card on your hand do you want to take with?", handCards);
                parameters.Append(CH.BuildParameters(idxs, handCards.IndexOf(handCard)));
                break;
            case "Clear table":
                method = "_cleartable";
                //TODO Five of spades will be renamed - change to new style
                parameters.Append(CH.BuildParameters(handCards.IndexOf("5 of Spades")));
                break;
        }

        var message = method + "_:_" + parameters;
        return message;
    }

    /// <summary>
    /// Method used to handle quitting the game. Prompts the user if they want to quit.
    /// </summary>
    /// <returns>A boolean indicating if the game should be quit or not</returns>
    private static bool QuitGame()
    {
        var confirmation = Prompt.Confirm("Are you sure you want to quit?");
        if (!confirmation) return false;
        Outbound.Broadcast($"{MyIp} has quit the game");
        Environment.Exit(1);

        return true;
    }

    /// <summary>
    /// Method used to filter the actions presented to the player.
    /// </summary>
    /// <param name="tableCards">The cards on the table</param>
    /// <param name="handCards">The cards in the player's hand</param>
    /// <returns>An array of actions to be used when populating player prompts</returns>
    private static string[] FilterPlayerActions(List<string> tableCards, out List<string> handCards)
    {
        string[] actions = ["Place a card", "Build", "Take", "Clear table", "QUIT"];
        if (!(tableCards.Count > 0))
            actions = actions.Where(action => action is not ("Build" or "Take")).ToArray();
        handCards = GameState.Players.Single(player => player.Name == MyIp).Hand
            .Select(card => card.ToString()).ToList();
        if (handCards.All(card => card != "5 of Spades"))
            actions = actions.Where(action => action is not ("Clear table")).ToArray();
        return actions;
    }

    /// <summary>
    /// Method used to extract the cards on the table to a human-readable representation.
    /// </summary>
    /// <returns>A list of strings representing the cards on the table</returns>
    private static List<string> ExtractTableCardsToList()
    {
        return GameState.Table.Piles.Select(pile =>
            string.Join(", ", pile.Pile.Cards.Select(card => card.ToString())) +
            (pile.Pile.Cards.Count > 1 ? $" ({pile.Values.Single()})" : "")).ToList();
    }

    /// <summary>
    /// Method used to stall the game while the lobby is being setup by the host.
    /// </summary>
    private static void AwaitGameStart()
    {
        while (GameState == null)
        {
            Thread.Sleep(500);
            Console.WriteLine("Waiting for game to start...");
        }
    }
    
    /// <summary>
    /// Method used to prompt the player which lobby (IP) they would like to connect to. 
    /// </summary>
    private static void JoinLobby()
    {
        var serverIp = Prompt.Input<string>("Enter the ip you want to connect to");
        Outbound.NewRecipient(serverIp);
        var rsaParams = RSA.ExportParameters(false);
        Outbound.Broadcast($"PUB{CH.ProtocolSplit}{Encoding.ASCII.GetString(rsaParams.Modulus)};{Encoding.ASCII.GetString(rsaParams.Exponent)}");
    }

    /// <summary>
    /// Method used to create or host a lobby and start the game when ready.
    /// </summary>
    /// <returns></returns>
    private static bool CreateLobby()
    {
        var ready = Prompt.Confirm("Are you ready to start the game?");
        if (!ready) return false;
        
        Outbound.Broadcast($"AES{Aes.Key};{Aes.IV}", EncryptionHandler.Type.RSA);
        
        GameState = new GameState(Outbound.Recipients.Select(sender => sender.IpAddress).Append(MyIp).Reverse()
            .ToList());
        Outbound.Broadcast($"GAMESTATE{CH.ProtocolSplit}{GameState.Serialize()}");
        Console.WriteLine(GameState.Serialize());
        //Console.WriteLine("GameState has been setup");
        //Console.Clear();
        GameState.DisplayGame(MyIp);
        return true;
    }

    /// <summary>
    /// Method to set up a receiver thread used to receive messages from other peers.
    /// A prerequisite to enabling peers in creating or joining lobbies. 
    /// </summary>
    private static void SetupReceiver()
    {
        Console.WriteLine($"I am {MyIp}");
        //string command;
        var receiver = new Thread(Inbound.Receiver);
        receiver.Start();
    }
}