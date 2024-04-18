﻿// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;
using Game;

namespace Peer;

public class Program
{
    public const int Port = 8000;
    //public static int GameState { get; set; }
    public static string MyIp = "172.29.0.10";
    public static GameState GameState { get; set; }

    public static void Main(string[] args)
    {
        // Get all IP addresses associated with the host
        var addresses = Dns.GetHostEntry(Dns.GetHostName()).AddressList;

        // Filter out IPv4 addresses
        var ipv4Addresses = addresses.Where(address => address.AddressFamily == AddressFamily.InterNetwork).ToArray();
        //MyIp = ipv4Addresses[0].ToString();

        Console.WriteLine($"I am {MyIp}");
        string command;
        var receiver = new Thread(Inbound.Receiver);
        receiver.Start();
        while (true)
        {
            Console.WriteLine("Do you want to >await< or create a connection >manually<?");
            command = Console.ReadLine();
            if (command.Equals("await"))
            {
                var approval = Console.ReadLine();
                if (approval.Equals("ok"))
                {
                    GameState = new GameState(Outbound.Senders.Select(sender => sender.Key).ToList());
                    GameState.Setup();
                    Outbound.Broadcast($"GAMESTATE:{GameState.Serialize()}");
                    Console.WriteLine("GameState has been setup");
                    break;
                }
            }

            if (!command.Equals("manually")) continue;
            Console.WriteLine("Enter the ip you want to manually connect to");
            var serverIp = Console.ReadLine();
            Outbound.NewSender(serverIp);
            break;
        }

        while (true)
        {
            var message = Console.ReadLine() ?? "";
            if (message == "QUIT") break;
            Outbound.Broadcast(message);
        }

        foreach (var sender in Outbound.Senders) sender.Value.Close();

        Environment.Exit(1);
    }
}