using System.Net.Sockets;
using System.Security.Cryptography;

namespace Peer;

public class Recipient
{
    public string IpAddress { get; set; }
    public TcpClient Client { get; set; }
    public RSA Rsa { get; set; }

    public Recipient(string ipAddress, TcpClient client)
    {
        IpAddress = ipAddress;
        Client = client;
    }

    public void SetPublicKey(byte[] modulus, byte[] exponent)
    {
        Rsa = RSA.Create();
        RSAParameters rsaParams = new RSAParameters
        {
            Modulus = modulus,
            Exponent = exponent
        };
        Rsa.ImportParameters(rsaParams);
    }
}