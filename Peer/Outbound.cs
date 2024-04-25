using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Game;

namespace Peer;

public class Outbound
{
    public static Dictionary<string, TcpClient> Senders = new();

    public static void Broadcast(string message)
    {
        var (method, parameters) = CommunicationHandler.GetPayload(message);
        if (method.StartsWith('_'))
        {
            if (MethodHandler.CallMethod(method, parameters))
            {
                Program.GameState.AdvanceTurn(method);
                Console.WriteLine(Program.GameState.DisplayGame(Program.MyIp));
                foreach (var tcpClient in Senders.Select(sender => sender.Value)) SendMessage(tcpClient, message);
            }
            else
            {
                Console.WriteLine("Your move was not accepted - try again!");
            }
        }
        else
        {
            foreach (var tcpClient in Senders.Select(sender => sender.Value)) SendMessage(tcpClient, message);
        }
    }

    private static void SendMessage(TcpClient client, string message)
    {
        var data = Encoding.ASCII.GetBytes(message);
        var stream = client.GetStream();
        stream.Write(data, 0, data.Length);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void NewSender(string ipAddress)
    {
        if (ipAddress.Equals(Program.MyIp) || Senders.ContainsKey(ipAddress)) return;
        var sender = new TcpClient(ipAddress, Program.Port);
        Senders.Add(ipAddress, sender);
        Console.WriteLine($"New sender added with ip: {ipAddress}");
    }
}