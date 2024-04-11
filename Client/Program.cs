// See https://aka.ms/new-console-template for more information

using System.Net.Sockets;
using System.Text;

const string serverIp = "127.0.0.1"; // Change to the server's IP
int port = 8000;
TcpClient client = new TcpClient(serverIp, port);
Console.WriteLine($"Connected to {serverIp}:{port}");

while (true)
{
    string message = Console.ReadLine();
    if (message == "QUIT")
        break;

    byte[] data = Encoding.ASCII.GetBytes(message);
    NetworkStream stream = client.GetStream();
    stream.Write(data, 0, data.Length);
}

client.Close();