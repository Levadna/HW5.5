using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Collections.Concurrent;
using TcpP43.Common;

Console.WriteLine("Sync Server");

string folder = @"C:\Users\asus\Documents\SyncServer";
Directory.CreateDirectory(folder);

int tcpPort = 7000;
int udpPort = 5001;
IPAddress ip = IPAddress.Parse("192.168.1.110");
ConcurrentDictionary<string, string> files = new ConcurrentDictionary<string, string>();
UdpClient udpClient = new UdpClient();
udpClient.EnableBroadcast = true;
IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, udpPort);
FileSystemWatcher watcher = new FileSystemWatcher(folder);
watcher.Filter = "*";
watcher.IncludeSubdirectories = true;
watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size;
watcher.Created += (sender, e) =>
{
    string name = Path.GetFileName(e.FullPath);
    files[name] = e.FullPath;
    SendMessage("Created", name);
};
watcher.Changed += (sender, e) =>
{
    string name = Path.GetFileName(e.FullPath);
    files[name] = e.FullPath;
    SendMessage("Changed", name);
};
watcher.Deleted += (sender, e) =>
{
    string name = Path.GetFileName(e.FullPath);
    files.TryRemove(name, out _);
    SendMessage("Deleted", name);
};
watcher.EnableRaisingEvents = true;

Console.WriteLine($"Watching folder: {folder}");
TcpListener tcpListener = new TcpListener(IPAddress.Any, tcpPort);
tcpListener.Start();
Console.WriteLine($"TCP server started on port {tcpPort}");
while (true)
{
    TcpClient client = await tcpListener.AcceptTcpClientAsync();
    try
    {
        using NetworkStream stream = client.GetStream();
        string name = await NetworkHelper.ReceiveStringAsync(stream);
        if (files.TryGetValue(name, out string path) && File.Exists(path))
        {
            await NetworkHelper.SendFileAsync(stream, path);
            Console.WriteLine($"Sent file: {name}");
        }
        else
        {
            Console.WriteLine($"File not found: {name}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
    finally
    {
        client.Close();
    }
}
void SendMessage(string type, string name)
{
    string message = $"{type}:{name}:{ip}:{tcpPort}";
    byte[] data = Encoding.UTF8.GetBytes(message);
    udpClient.Send(data, data.Length, endPoint);
    Console.WriteLine($"Sent: {message}");
}