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
    
    [Fact]
    public void RSAEncrypt_WithPrefix_CanBeSplitAndDecrypted()
    {
        var rsa = RSA.Create();
        var msg = "Hello, World!";
    
        // Encrypt the message
        var encryptedBytes = rsa.Encrypt(Encoding.ASCII.GetBytes(msg), RSAEncryptionPadding.Pkcs1);
    
        // Combine the prefix with the encrypted message
        var sendString = Encoding.ASCII.GetBytes($"RSA:_:{Convert.ToBase64String(encryptedBytes)}");

        var recieveString = sendString;
    
        // Split the combined string
        var splitSendString = Encoding.ASCII.GetString(recieveString).Split(":_:");
        var encryptedMsg = splitSendString.Last();
    
        // Decrypt the message
        var decryptedBytes = rsa.Decrypt(Convert.FromBase64String(encryptedMsg), RSAEncryptionPadding.Pkcs1);
        var decryptedMsg = Encoding.ASCII.GetString(decryptedBytes);
    
        // Verify that the decrypted message matches the original
        Assert.Equal(msg, decryptedMsg);
    }

}