using System.Security.Cryptography;
using System.Text;

namespace Peer;

public static class EncryptionHandler
{
    public enum Type
    {
        None,
        RSA,
        Aes
    }

    public const string Split = ":_:";

    public static string Decrypt(string input)
    {
        var splitMessage = input.Split(Split);
        var encryptionType = Enum.Parse<Type>(splitMessage.First());
        var message = Convert.FromBase64String(string.Join("", splitMessage.Skip(1).ToArray()));

        var bytes = encryptionType switch
        {
            Type.None => message,
            Type.RSA => Program.RSA.Decrypt(message, RSAEncryptionPadding.Pkcs1),
            Type.Aes => Program.Aes.DecryptCbc(message, Program.Aes.IV),
            _ => throw new ArgumentOutOfRangeException()
        };


        return Encoding.ASCII.GetString(bytes);
    }
}