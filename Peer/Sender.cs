using System.Net.Sockets;

namespace Peer;

public class Sender
{
    public string IpAddress { get; set; }
    public TcpClient Client { get; set; }
    public byte[] PublicKey { get; init; }

    public Sender(string ipAddress, TcpClient client)
    {
        IpAddress = ipAddress;
        Client = client;
        PublicKey = [];
    }
}