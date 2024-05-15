using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;


namespace Peer;

public class Outbound
{
    //public static Dictionary<string, TcpClient> Senders = new();
    public static List<Recipient> Recipients = [];

    // TODO: How do we show what encryption is used for the message, for the 'client' to know how to decrypt?
    public static void Broadcast(string message, EncryptionHandler.Type encryption = EncryptionHandler.Type.None)
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

    private static void SendMessage(string message, EncryptionHandler.Type encryption)
    {
        foreach (var recipient in Recipients)
        {
            var messageBytes = Encoding.ASCII.GetBytes(message);
            switch (encryption)
            {
                case EncryptionHandler.Type.None:
                    SendMessageImpl(recipient.Client,
                        Encoding.ASCII.GetBytes(
                            $"None{EncryptionHandler.Split}{Convert.ToBase64String(messageBytes)}"));
                    break;
                case EncryptionHandler.Type.RSA:
                    SendMessageImpl(recipient.Client,
                        Encoding.ASCII.GetBytes(
                            $"RSA{EncryptionHandler.Split}{Convert.ToBase64String(recipient.Rsa.Encrypt(messageBytes, RSAEncryptionPadding.Pkcs1))}"));
                    break;
                case EncryptionHandler.Type.Aes:
                    SendMessageImpl(recipient.Client,
                        Encoding.ASCII.GetBytes(
                            $"Aes{EncryptionHandler.Split}{Convert.ToBase64String(Program.Aes.EncryptCbc(messageBytes, Program.Aes.IV))}"));
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