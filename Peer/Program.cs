// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Peer;

public class Program
{
    public static int gameState = 0;
    
    public static void Main(string[] args)
    {
        Thread receiver = new Thread(Receiver);
        //Thread client = new Thread(Client);
        receiver.Start();
        //client.Start();
        Sender();
    }

    static void Sender()
    {
        const string serverIp = "127.0.0.1"; // Change to the server's IP
        int port = 8000;
        TcpClient client = new TcpClient(serverIp, port);
        Console.WriteLine($"Connected to {serverIp}:{port}");

        while (true)
        {
            string message = Console.ReadLine();
            if (message == "QUIT")
                break;
            else if (message.StartsWith("ADD"))
                gameState += Int32.Parse(message.Split(":").Last());
            
            
            
            byte[] data = Encoding.ASCII.GetBytes(message);
            NetworkStream stream = client.GetStream();
            stream.Write(data, 0, data.Length);
        }

        client.Close();
        Environment.Exit(0);
    }

    static void Receiver()
    {
        int port = 8000;
        TcpListener server = new TcpListener(IPAddress.Any, port);
        server.Start();
        Console.WriteLine($"Server started on port {port}");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
    
            Console.WriteLine($"Client connected. {client.Client.RemoteEndPoint}");
            Thread clientThread = new Thread(HandleClientComm);
            clientThread.Start(client);
        }
    }
    
    static void HandleClientComm(object client)
    {
        TcpClient tcpClient = (TcpClient)client;
        NetworkStream clientStream = tcpClient.GetStream();

        byte[] message = new byte[4096];
        int bytesRead;

        while (true)
        {
            bytesRead = 0;

            try
            {
                bytesRead = clientStream.Read(message, 0, 4096);
            }
            catch
            {
                break;
            }

            if (bytesRead == 0)
            {
                break;
            }

            string dataReceived = Encoding.ASCII.GetString(message, 0, bytesRead);
            Console.WriteLine($"Received: {dataReceived}");
            if (dataReceived.StartsWith("ADD")) gameState += Int32.Parse(dataReceived.Split(":").Last());
            Console.WriteLine($"Game state is {gameState}");
            // Process the message here
        }

        tcpClient.Close();
    }
}