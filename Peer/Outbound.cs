using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;


namespace Peer;

public class Outbound
{
    //public static Dictionary<string, TcpClient> Senders = new();
    public static List<Recipient> Recipients = [];

    public static void Broadcast(string message, string encryption = "none")
    {
        
        Console.Clear();
        var (method, parameters) = CH.GetPayload(message);
        if (method.StartsWith('_'))
        {
            if (MethodHandler.CallMethod(method, parameters))
            {
                Program.GameState.AdvanceTurn(method);
                Program.GameState.DisplayGame(Program.MyIp);
                foreach (var tcpClient in Recipients.Select(sender => sender.Client)) SendMessage(tcpClient, Encoding.ASCII.GetBytes(message));
            }
            else
            {
                Console.WriteLine("Your move was not accepted - try again!");
            }
        }
        else
        {
            foreach (var recipient in Recipients)
            {
                SendMessage(recipient.Client, recipient.Rsa.Encrypt(Encoding.ASCII.GetBytes(message), RSAEncryptionPadding.Pkcs1));
            }
        }
    }

    private static void SendMessage(TcpClient client, byte[] data)
    {
        var stream = client.GetStream();
        stream.Write(data, 0, data.Length);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void NewRecipient(string ipAddress, byte[]? publicKey = null)
    {
        if (ipAddress.Equals(Program.MyIp) || Recipients.Any(sender => sender.IpAddress == ipAddress)) return;
        var client = new TcpClient(ipAddress, Program.Port);
        Recipients.Add(new Recipient(ipAddress, client));
        Console.WriteLine($"New sender added with ip: {ipAddress}");
    }
}