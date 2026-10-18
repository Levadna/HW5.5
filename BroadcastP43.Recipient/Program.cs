using System.Net;
using System.Net.Sockets;
using System.Text;

Console.WriteLine("Broadcast Recipient");

UdpClient udpClient = new UdpClient(5000);

var port = 5000;
var endPoint = new IPEndPoint(IPAddress.Any, port);

Console.WriteLine($"Listening for broadcast messages on port {port}...");

while (true)
{
    var receivedResults = await udpClient.ReceiveAsync();
    string receivedMessage = Encoding.UTF8.GetString(receivedResults.Buffer);
    Console.WriteLine($"Received: {receivedMessage}");

    string[] parts = receivedMessage.Split(':');
    string ip = parts[0];
    int tcpPort = int.Parse(parts[1]);
    string id = parts[2];

    TcpClient tcpClient = new TcpClient();
    await tcpClient.ConnectAsync(ip, tcpPort);
    NetworkStream stream = tcpClient.GetStream();

    byte[] data = Encoding.UTF8.GetBytes(id);
    await stream.WriteAsync(data, 0, data.Length);

    byte[] buffer = new byte[1024];
    int count = await stream.ReadAsync(buffer, 0, buffer.Length);

    string text = Encoding.UTF8.GetString(buffer, 0, count);
    Console.WriteLine($"Message: {text}");
    tcpClient.Close();
}