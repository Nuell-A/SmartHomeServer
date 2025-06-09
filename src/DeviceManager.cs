using System.Net.Sockets;
using System.Text;
using System.Text.Json;

/*
Handles connections with accepted connections.
*/

class DeviceManager
{

    public async void HandleClient(Socket conn, TCPSocket tcpSocket)
    {
        // Buffer to check message length
        byte[] bufferSize = new byte[4]; // 4 for int byte size
        byte[] buffer = new byte[await tcpSocket.ReceiveLength(bufferSize, conn)]; // Now that we have the size of buffer needed

        string message = await tcpSocket.Receive(buffer, conn);
        JSONMessage jsonObject = JsonSerializer.Deserialize<JSONMessage>(message);

        Console.WriteLine($"{jsonObject.source}\n{jsonObject.type}");

        Console.WriteLine("------------------------");

        Console.WriteLine(message);

        // Create JSON object
        JSONMessage response = new()
        {
            source = "device",
            type = "information",
            device_ip = "clientIp.ToString()",
            state = new Dictionary<string, string>
            {
                { "relay1", "on" }
            },
            timestamp = "temporary"
        };

        string messageJson = JsonSerializer.Serialize(response); // Serialize
        await SendLength(messageJson, conn);
        await Send(messageJson, conn);
    }

    static async Task Send(string message, Socket conn)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);

        Console.WriteLine("Sending message...");
        await conn.SendAsync(messageBytes, SocketFlags.None);
        Console.WriteLine("Message sent.");
    }

    static async Task SendLength(string message, Socket conn)
    {
        byte[] messageBytesLength = Encoding.UTF8.GetBytes(message);
        int length = messageBytesLength.Length;
        byte[] lengthPrefix = BitConverter.GetBytes(length);


        Console.WriteLine("Sending length...");
        _ = await conn.SendAsync(lengthPrefix, SocketFlags.None);
        Console.WriteLine("Length sent.");
    }
}