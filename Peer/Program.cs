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
    public static string MyIp;

    public static void Main(string[] args)
    {
        MyIp = Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();
        var command = "";
        var receiver = new Thread(Receiver);
        //Thread client = new Thread(Client);
        receiver.Start();
        //client.Start();
        //192.168.155.21
        //Console.Write("IP Address: ");
        // var senderIp = client.Client.RemoteEndPoint!.ToString()!.Split(":").First();
        while (true)
        {
            Console.WriteLine("Do you want to >await< or create a connection >manually<?");
            command = Console.ReadLine();
            if (command.Equals("await"))
            {
                var approval = Console.ReadLine();
                if (approval.Equals("ok"))
                {
                    Console.WriteLine("Ready to send messages");
                    break;
                }
            }

            if (!command.Equals("manually")) continue;
            Console.WriteLine("Enter the ip you want to manually connect to");
            var serverIp = Console.ReadLine();
            NewSender(serverIp);
            break;
        }

        while (true)
        {
            var message = Console.ReadLine() ?? "";
            if (message == "QUIT") break;
            Broadcast(Senders.Select(x => x.Value).ToList(), message);
        }

        foreach (var sender in Senders)
        {
            sender.Value.Close();
        }

        Environment.Exit(1);
    }

    private static void Broadcast(List<TcpClient> clients, string message)
    {
        if (message.StartsWith("ADD")) GameState += int.Parse(message.Split(":").Last());
        foreach (var tcpClient in clients)
        {
            SendMessage(tcpClient, message);
        }
    }

    private static void SendMessage(TcpClient client, string message)
    {
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
            var message = "IP:" + string.Join(";", Senders.Select(x => x.Key));
            Broadcast(Senders.Select(x => x.Value).ToList(), message);
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
            var remoteIp = tcpClient.Client.RemoteEndPoint.ToString().Split(":").First();
            Console.WriteLine($"{remoteIp} Received: {dataReceived}");
            if (dataReceived.StartsWith("ADD")) GameState += int.Parse(dataReceived.Split(":").Last());
            if (dataReceived.StartsWith("IP"))
            {
                var ips = dataReceived.Split(":").Last().Split(";");
                foreach (var ip in ips)
                {
                    if (!Senders.ContainsKey(ip))
                    {
                        NewSender(ip);
                    }
                }
            }

            Console.WriteLine($"Game state is {GameState}");
        }

        tcpClient.Close();
    }

    public static void NewSender(string ipAddress)
    {
        if (ipAddress.Equals(MyIp)) return;
        var sender = new TcpClient(ipAddress, Port);
        Senders.Add(ipAddress, sender);
        Console.WriteLine($"New sender added with ip: {ipAddress}");
    }
}