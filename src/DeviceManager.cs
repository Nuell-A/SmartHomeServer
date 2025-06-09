using System.Net.Sockets;
using System.Text;

/*
Acts and "client" handler, meaning this will handle connections with micro-controllers.
*/

class DeviceManager
{
    const string EOM = "<|EOM|>";

    public async void HandleClient(Socket conn, TCPSocket tcpSocket)
    {
        byte[] buffer = new byte[1024];
        string message = await tcpSocket.Receive(buffer, conn);

        Console.WriteLine(message);

        string response = "Message received." + EOM;

        await Send(response, conn);
    }

    static async Task Send(string message, Socket conn)
{
    byte[] messageBytes = Encoding.UTF8.GetBytes(message);

    Console.WriteLine("Sending message...");
    await conn.SendAsync(messageBytes, 0);
    Console.WriteLine("Message sent.");
}
}