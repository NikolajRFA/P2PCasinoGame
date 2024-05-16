using System.Net.Sockets;
using System.Security.Cryptography;

namespace Peer;

public class Recipient
{
    public Recipient(string ipAddress, TcpClient client)
    {
        IpAddress = ipAddress;
        Client = client;
    }

    public string IpAddress { get; set; }
    public TcpClient Client { get; set; }
    public RSA Rsa { get; set; }

    public void SetPublicKey(byte[] modulus, byte[] exponent)
    {
        Rsa = RSA.Create();
        // Do not refactor to use object initializer, for some reason it does not work here!
        RSAParameters rsaParams = new();
        rsaParams.Exponent = exponent;
        rsaParams.Modulus = modulus;
        Rsa.ImportParameters(rsaParams);
    }
}