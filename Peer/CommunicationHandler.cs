namespace Peer;

public class CommunicationHandler : ICommunicationHandler
{
    public const string ProtocolSplit = "(;;)";
    public static void Ips(List<string> ips)
    {
        foreach (var ip in ips)
            if (!Outbound.Senders.ContainsKey(ip))
                Outbound.NewSender(ip);
    }

    public static string GetMethod(string data)
    {
        return data.Split(ProtocolSplit).First();
    }

    public static List<int> GetParameters(string data)
    {
        return data.Split(ProtocolSplit).Last().Split(";").ToList().Select(int.Parse).ToList();
    }

    public static List<string> GetIps(string parameters)
    {
        return parameters.Split(";").ToList();
    }

    public static (string, List<int>) GetPayload(string data)
    {
        return (GetMethod(data), GetParameters(data));
    }
}