namespace Peer;

public class CH : ICommunicationHandler
{
    public const string ProtocolSplit = "_:_";

    public static void Ips(List<string> ips)
    {
        foreach (var ip in ips)
            if (Outbound.Recipients.All(sender => sender.IpAddress != ip))
                Outbound.NewRecipient(ip);
    }

    public static string GetMethod(string data)
    {
        return data.Split(ProtocolSplit).First();
    }

    public static string BuildParameters(IEnumerable<int> listValues, params int[] values)
    {
        return $"[{string.Join(",", listValues)}];{BuildParameters(values)}";
    }

    public static string BuildParameters(params int[] values)
    {
        return $"{string.Join(";", values)}";
    }
    public static List<string> GetParameters(string data)
    {
        // ["[1,2,3]", "1", "2"]
        return data.Split(ProtocolSplit).Last().Split(";").ToList();
    }

    public static List<string> GetIps(string parameters)
    {
        return parameters.Split(";").ToList();
    }

    public static (string, List<string>) GetPayload(string data)
    {
        return (GetMethod(data), GetParameters(data));
    }
    
    public static void EncryptMessage(){}
    public static void DecryptMessage(){}

    public enum EncryptionType
    {
        None,
        RSA,
        Aes
    }
}