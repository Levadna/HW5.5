using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

Console.WriteLine("Broadcast Sender");

Dictionary<int, string> messages = new Dictionary<int, string>();

string[] texts =
{
    "Server started",
    "New client connected",
    "CPU usage is high",
    "Backup completed",
    "System temperature is normal",
    "Database synchronized"
};

UdpClient udpClient = new UdpClient();
udpClient.EnableBroadcast = true;

var port = 5000;
var broadcastAdress = IPAddress.Broadcast;
IPEndPoint endPoint = new IPEndPoint(broadcastAdress, port);

_ = Task.Run(async () =>
{
    TcpListener tcpListener = new TcpListener(IPAddress.Any, 7000);
    tcpListener.Start();
    Console.WriteLine("TCP server started on port 7000");
    while (true)
    {
        TcpClient client = await tcpListener.AcceptTcpClientAsync();
        Console.WriteLine("New client connected");
        NetworkStream stream = client.GetStream();

        byte[] buffer = new byte[1024];
        int count = await stream.ReadAsync(buffer, 0, buffer.Length);

        string receivedId = Encoding.UTF8.GetString(buffer, 0, count);
        if (int.TryParse(receivedId, out int id) && messages.ContainsKey(id))
        {
            string text = messages[id];
            byte[] data = Encoding.UTF8.GetBytes(text);
            await stream.WriteAsync(data, 0, data.Length);
            Console.WriteLine($"Sent message: {text}");
        }
        client.Close();
    }
});

while (true)
{
    string ip = "192.168.1.110";
    int tcpPort = 7000;
    int id = Random.Shared.Next(100, 1000);
    string text = texts[Random.Shared.Next(texts.Length)];
    messages[id] = text;
    string message = $"{ip}:{tcpPort}:{id}";
    byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
    udpClient.Send(data, data.Length, endPoint);
    Console.WriteLine($"Sent: {message}");
    await Task.Delay(Random.Shared.Next(2000, 7001));
}

