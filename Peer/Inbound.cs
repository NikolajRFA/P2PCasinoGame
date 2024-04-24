using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Game;

namespace Peer;

public class Inbound
{
    public static void Receiver()
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
            if (!Outbound.Senders.ContainsKey(senderIp))
                Outbound.NewSender(senderIp);
            var message = "IP" + CommunicationHandler.ProtocolSplit +
                          string.Join(";", Outbound.Senders.Select(x => x.Key));
            Outbound.Broadcast(message);
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
            // Handle commands
            var (method, data) = CommunicationHandler.GetPayload(dataReceived);

            switch (method)
            {
                case not null when method.StartsWith("IP"):
                    CommunicationHandler.Ips(CommunicationHandler.GetIps(string.Join(";", data)));
                    break;
                case not null when method.StartsWith("GAMESTATE"):
                    Console.WriteLine(data);
                    var options = new JsonSerializerOptions
                    {
                        IncludeFields = true,
                    };
                    Program.GameState = JsonSerializer.Deserialize<GameState>(data[0], options)!;
                    break;
                case not null when method.StartsWith("_"):
                    MethodHandler.CallMethod(method, data);
                    Console.WriteLine(Program.GameState.DisplayGame(Program.MyIp));
                    break;
                default:
                    Console.WriteLine("Unrecognized input pattern");
                    break;
            }


            //Console.WriteLine($"Game state is {Program.GameState.Serialize()}");
        }

        tcpClient.Close();
    }
}