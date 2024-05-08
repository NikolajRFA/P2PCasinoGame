// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Game;
using Sharprompt;

namespace Peer;

public class Program
{
    public const int Port = 8000;

    //public static int GameState { get; set; }
    public static string MyIp = "172.29.0.13";
    public static GameState GameState { get; set; }

    public static void Main(string[] args)
    {
        SetupReceiver();
        
        SetupLobby();

        AwaitGameStart();

        HandleTurn();
    }

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

    private static void HandleTurn()
    {
        while (true)
        {
            if (GameState.Players[GameState.CurrentPlayer].Name == MyIp)
            {
                var tableCards = ExtractTableCardsToList();
                var actions = CreatePlayerActions(tableCards, out var handCards);
               
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

    private static string MakeMove(string[] actions, List<string> handCards, List<string> tableCards)
    {
        var input = Prompt.Select("Make your move", actions);
        if (input == "QUIT")
        {
            input = QuitGame(input, actions);
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

    private static string QuitGame(string input, string[] actions)
    {
        var confirmation = Prompt.Confirm("Are you sure you want to quit?");
        if (confirmation)
        {
            Outbound.Broadcast($"{MyIp} has quit the game");
            Environment.Exit(1);
        }
        else
        {
            input = Prompt.Select("Make your move", actions);
        }

        return input;
    }

    private static string[] CreatePlayerActions(List<string> tableCards, out List<string> handCards)
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

    private static List<string> ExtractTableCardsToList()
    {
        return GameState.Table.Piles.Select(pile =>
            string.Join(", ", pile.Cards.Select(card => card.ToString())) +
            (pile.Cards.Count > 1 ? $" ({pile.Values.Single()})" : "")).ToList();
    }

    private static void AwaitGameStart()
    {
        while (GameState == null)
        {
            Thread.Sleep(500);
            Console.WriteLine("Waiting for game to start...");
        }
    }

    private static void JoinLobby()
    {
        var serverIp = Prompt.Input<string>("Enter the ip you want to connect to");
        Outbound.NewSender(serverIp);
    }

    private static bool CreateLobby()
    {
        var ready = Prompt.Confirm("Are you ready to start the game?");
        if (!ready) return false;
        GameState = new GameState(Outbound.Senders.Select(sender => sender.Key).Append(MyIp).Reverse()
            .ToList());
        Outbound.Broadcast($"GAMESTATE{CH.ProtocolSplit}{GameState.Serialize()}");
        Console.WriteLine(GameState.Serialize());
        //Console.WriteLine("GameState has been setup");
        //Console.Clear();
        GameState.DisplayGame(MyIp);
        return true;

    }

    private static void SetupReceiver()
    {
        // Get all IP addresses associated with the host
        var addresses = Dns.GetHostEntry(Dns.GetHostName()).AddressList;

        // Filter out IPv4 addresses
        var ipv4Addresses = addresses.Where(address => address.AddressFamily == AddressFamily.InterNetwork).ToArray();
        //MyIp = ipv4Addresses[0].ToString();

        Console.WriteLine($"I am {MyIp}");
        //string command;
        var receiver = new Thread(Inbound.Receiver);
        receiver.Start();
    }
}