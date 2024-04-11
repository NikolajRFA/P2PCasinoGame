// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Peer;

public class Program
{
    public const int Port = 8000;
    public static int GameState;
    public static Dictionary<string, TcpClient> Senders = new();

    public static void Main(string[] args)
    {
        var receiver = new Thread(Receiver);
        //Thread client = new Thread(Client);
        receiver.Start();
        //client.Start();
        //192.168.155.21
        Console.Write("IP Address: ");
        var serverIp = Console.ReadLine();
        NewSender(serverIp);

        while (true)
        {
            var message = Console.ReadLine() ?? "";
            if (message == "QUIT") break;
            foreach (var sender in Senders)
            {
                SendMessage(sender.Value, message);
            }
        }

        foreach (var sender in Senders)
        {
            sender.Value.Close();
        }
        Environment.Exit(1);
    }

    private static void SendMessage(TcpClient client, string message)
    {
        if (message.StartsWith("ADD"))
            GameState += int.Parse(message.Split(":").Last());


        var data = Encoding.ASCII.GetBytes(message);
        var stream = client.GetStream();
        stream.Write(data, 0, data.Length);
    }

    private static void Receiver()
    {
        var port = 8000;
        var server = new TcpListener(IPAddress.Any, port);
        server.Start();
        Console.WriteLine($"Server started on port {port}");

        while (true)
        {
            var client = server.AcceptTcpClient();

            Console.WriteLine($"Client connected. {client.Client.RemoteEndPoint}");
            var senderIp = client.Client.RemoteEndPoint!.ToString()!.Split(":").First();
            if (!Senders.ContainsKey(senderIp))
                NewSender(senderIp);
            foreach (var sender in Senders)
            {
                SendMessage(sender.Value, string.Join(":", Senders.Select(x => x.Key)));
            }
            var clientThread = new Thread(HandleClientComm);
            clientThread.Start(client);
        }
    }

    private static void HandleClientComm(object client)
    {
        var tcpClient = (TcpClient)client;
        var clientStream = tcpClient.GetStream();

        var message = new byte[4096];
        int bytesRead;

        while (true)
        {
            bytesRead = 0;

            try
            {
                bytesRead = clientStream.Read(message, 0, 4096);
            }
            catch
            {
                break;
            }

            if (bytesRead == 0) break;

            var dataReceived = Encoding.ASCII.GetString(message, 0, bytesRead);
            Console.WriteLine($"Received: {dataReceived}");
            if (dataReceived.StartsWith("ADD")) GameState += int.Parse(dataReceived.Split(":").Last());
            Console.WriteLine($"Game state is {GameState}");
        }

        tcpClient.Close();
    }

    public static void NewSender(string ipAddress)
    {
        var sender = new TcpClient(ipAddress, Port);
        try
        {
            Senders.Add(ipAddress, sender);
        }
        catch
        {
            Console.WriteLine($"{ipAddress} is already a sender!");
        }
    }
}