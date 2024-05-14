using System.Security.Cryptography;
using System.Text;

namespace Peer;

public class EncryptionHandler
{
    public const string Split = ":_:";
    
    public enum Type
    {
        None,
        RSA,
        Aes
    }

    public string Decrypt(string input)
    {
        var splitMessage = input.Split(Split);
        var encryptionType = Enum.Parse<Type>(splitMessage.First());
        var message = Encoding.ASCII.GetBytes(string.Join("", splitMessage[1..^1]));

        byte[] bytes = [];
        switch (encryptionType)
        {
            case Type.None:
                break;
            case Type.RSA:
                bytes = Program.RSA.Decrypt(message, RSAEncryptionPadding.Pkcs1);
                break;
            case Type.Aes:
                bytes = Program.Aes.DecryptCbc(message, Program.Aes.IV);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return Encoding.ASCII.GetString(bytes);
    }
}

