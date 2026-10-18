using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using TcpP43.Common;

Console.WriteLine("Sync Client");

int udpPort = 5001;
string folder = @"C:\Users\asus\Documents\SyncClient";
Directory.CreateDirectory(folder);

UdpClient client = new UdpClient(udpPort);
Console.WriteLine($"Listening for broadcast messages on port {udpPort}...");
while (true)
{
    var receivedResults = await client.ReceiveAsync();
    string receivedMessage = Encoding.UTF8.GetString(receivedResults.Buffer);
    Console.WriteLine($"Received: {receivedMessage}");

    string[] parts = receivedMessage.Split(':');
    if (parts.Length != 4)
        continue;
    string type = parts[0];
    string name = parts[1];
    string ip = parts[2];
    int port = int.Parse(parts[3]);
    try
    {
        if (type == "Created" || type == "Changed")
        {
            await Task.Delay(300);
            TcpClient tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(ip, port);
            using NetworkStream stream = tcpClient.GetStream();
            await NetworkHelper.SendStringAsync(stream, name);
            await NetworkHelper.ReceiveFileAsync(stream, folder);
            Console.WriteLine($"File received: {name}");

            tcpClient.Close();
        }
        else if (type == "Deleted")
        {
            string filePath = Path.Combine(folder, name);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Console.WriteLine($"File deleted: {name}");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}