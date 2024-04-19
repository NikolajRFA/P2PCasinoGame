using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace Peer;

public class Outbound
{
    public static Dictionary<string, TcpClient> Senders = new();

    public static void Broadcast(string message)
    {
        //if (message.StartsWith("ADD")) Program.GameState += int.Parse(message.Split(":").Last());
        foreach (var tcpClient in Senders.Select(sender => sender.Value)) SendMessage(tcpClient, message);
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