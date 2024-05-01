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
        // Get all IP addresses associated with the host
        var addresses = Dns.GetHostEntry(Dns.GetHostName()).AddressList;

        // Filter out IPv4 addresses
        var ipv4Addresses = addresses.Where(address => address.AddressFamily == AddressFamily.InterNetwork).ToArray();
        //MyIp = ipv4Addresses[0].ToString();

        Console.WriteLine($"I am {MyIp}");
        //string command;
        var receiver = new Thread(Inbound.Receiver);
        receiver.Start();
        while (true)
        {
            //Console.WriteLine("Do you want to >await< or create a connection >manually<?");
            var command = Prompt.Select("Do you want to host or join?", ["host", "join"]);
            if (command.Equals("host"))
            {
                var ready = Prompt.Confirm("Are you ready to start the game?");
                if (ready)
                {
                    GameState = new GameState(Outbound.Senders.Select(sender => sender.Key).Append(MyIp).Reverse()
                        .ToList());
                    Outbound.Broadcast($"GAMESTATE{CommunicationHandler.ProtocolSplit}{GameState.Serialize()}");
                    //Console.WriteLine(GameState.Serialize());
                    //Console.WriteLine("GameState has been setup");
                    Console.Clear();
                    Console.WriteLine(GameState.DisplayGame(MyIp));
                    break;
                }
            }

            if (!command.Equals("join")) continue;
            var serverIp = Prompt.Input<string>("Enter the ip you want to connect to");
            Outbound.NewSender(serverIp);
            break;
        }

        while (GameState == null)
        {
            Thread.Sleep(500);
            Console.WriteLine("Waiting for game to start...");
        }

        while (true)
        {
            if (GameState.Players[GameState.CurrentPlayer].Name == MyIp)
            {
                string[] actions = ["Place a card", "Build", "Take", "Clear table", "QUIT"];
                var tableCards = GameState.Table.Cards.Select(pile =>
                    string.Join(", ", pile.Key.Cards.Select(card => card.ToString())) +
                    (pile.Key.Cards.Count > 1 ? $" ({pile.Value.Single()})" : "")).ToList();
                var handCards = GameState.Players.Single(player => player.Name == MyIp).Hand
                    .Select(card => card.ToString()).ToList();
                List<string> idxs = [];
                var input = Prompt.Select<string>("Make your move", actions);
                if (input == "QUIT") break;
                var method = "";
                var parameters = new StringBuilder();
                switch (input)
                {
                    case "Place a card":
                        method = "_placecard";
                        parameters.Append(Prompt.Input<string>("What card do you want to place?"));
                        break;
                    case "Build":
                        method = "_build";
                        parameters.Append('[');
                        var buildCards = Prompt.MultiSelect("Where do you want to build on the table?", tableCards);
                        idxs.AddRange(buildCards.Select(card => tableCards.IndexOf(card).ToString()));
                        parameters.Append(string.Join(",", idxs));
                        parameters.Append("];");
                        var handCard = Prompt.Select("Which card on your hand do you want to build with?", handCards);
                        parameters.Append(handCards.IndexOf(handCard));
                        parameters.Append(';');
                        parameters.Append(Prompt.Input<string>("What is the value of the building?"));
                        break;
                    case "Take":
                        method = "_take";
                        parameters.Append('[');
                        var takeCards = Prompt.MultiSelect("What do you want to take on the table?", tableCards);
                        idxs.AddRange(takeCards.Select(card => tableCards.IndexOf(card).ToString()));
                        parameters.Append(string.Join(",", idxs));
                        parameters.Append("];");
                        handCard = Prompt.Select("Which card on your hand do you want to take with?", handCards);
                        parameters.Append(handCards.IndexOf(handCard));
                        break;
                    case "Clear table":
                        method = "_cleartable";
                        parameters.Append(Prompt.Input<string>("In what position do you hold five of spades?"));
                        break;
                }

                var message = method + "_:_" + parameters;
                Console.WriteLine(message);
                Outbound.Broadcast(message);
            }
            else
            {
                Console.WriteLine("Wait for your turn...");
                Thread.Sleep(5000);
            }
        }
        /*
        while (true)
        {
            var message = Console.ReadLine() ?? "";
            if (message == "QUIT") break;
            Outbound.Broadcast($"{message}");

        }

        foreach (var sender in Outbound.Senders) sender.Value.Close();

        Environment.Exit(1);
        */
    }
}