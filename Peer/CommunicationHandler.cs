namespace Peer;

public class CommunicationHandler : ICommunicationHandler
{
    public const string ProtocolSplit = "_:_";

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
}