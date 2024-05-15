using System.Security.Cryptography;
using System.Text;
using Peer;

namespace TestProject1;

public class SecurityTests
{
    [Fact]
    public void Mockup_RSAToShareAesKeyFlow_Success()
    {
        var hostRsa = RSA.Create();
        var hostAes = Aes.Create();
        var joinRsa = RSA.Create();
        var joinAes = Aes.Create();

        hostRsa.ImportParameters(joinRsa.ExportParameters(false));

        joinAes.Key = joinRsa.Decrypt(
            hostRsa.Encrypt(hostAes.Key, RSAEncryptionPadding.Pkcs1),
            RSAEncryptionPadding.Pkcs1
        );
        joinAes.IV = joinRsa.Decrypt(
            hostRsa.Encrypt(hostAes.IV, RSAEncryptionPadding.Pkcs1),
            RSAEncryptionPadding.Pkcs1
        );

        var msg = "Hello, World!";
        var encryptedMsg = hostAes.EncryptCbc(Encoding.ASCII.GetBytes(msg), hostAes.IV);
        var decryptedMsg = joinAes.DecryptCbc(encryptedMsg, joinAes.IV);
        Assert.Equal(msg, Encoding.ASCII.GetString(decryptedMsg));
    }

    [Fact]
    public void Decrypt_NoEncryption_ReturnsMessage()
    {
        var msg = "Hello, World!";
        var decryptedMsg = EncryptionHandler.Decrypt($"None{EncryptionHandler.Split}{msg}");
        Assert.Equal(msg, decryptedMsg);
    }
}