using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;


namespace Peer;

public class Outbound
{
    //public static Dictionary<string, TcpClient> Senders = new();
    public static List<Recipient> Recipients = [];

    public static void Broadcast(string message, CH.EncryptionType encryption = CH.EncryptionType.None)
    {
        Console.Clear();
        var (method, parameters) = CH.GetPayload(message);
        if (method.StartsWith('_'))
        {
            if (MethodHandler.CallMethod(method, parameters))
            {
                Program.GameState.AdvanceTurn(method);
                Program.GameState.DisplayGame(Program.MyIp);
                SendMessage(message, encryption);
            }
            else
            {
                Console.WriteLine("Your move was not accepted - try again!");
            }
        }
        else
        {
            SendMessage(message, encryption);
        }
    }

    private static void SendMessage(string message, CH.EncryptionType encryption)
    {
        foreach (var recipient in Recipients)
        {
            var messageBytes = Encoding.ASCII.GetBytes(message);
            switch (encryption)
            {
                case CH.EncryptionType.None:
                    SendMessageImpl(recipient.Client, messageBytes);
                    break;
                case CH.EncryptionType.RSA:
                    SendMessageImpl(recipient.Client, recipient.Rsa.Encrypt(messageBytes, RSAEncryptionPadding.Pkcs1));
                    break;
                case CH.EncryptionType.Aes:
                    SendMessageImpl(recipient.Client, Program.Aes.EncryptCbc(messageBytes, Program.Aes.IV));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(encryption), encryption, null);
            }
        }
    }

    private static void SendMessageImpl(TcpClient client, byte[] data)
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