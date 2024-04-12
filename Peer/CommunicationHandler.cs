namespace Peer;

public class CommunicationHandler : ICommunicationHandler
{
    public static void Add(int amount)
    {
        Program.GameState += amount;
    }

    public static void Ips(List<string> ips)
    {
        foreach (var ip in ips)
        {
            if (!Program.Senders.ContainsKey(ip))
            {
                Program.NewSender(ip);
            }
        }
    }

    public static string GetMethod(string data)
    {
        return data.Split(":").First();
    }

    public static string GetParameters(string data)
    {
        return data.Split(":").First();
    }

    public static List<string> GetListFromParameters(string parameters)
    {
        return parameters.Split(";").ToList();
    }

    public static (string, string) GetPayload(string data)
    {
        return (GetMethod(data), GetParameters(data));
    }
}