using System.Net.Sockets;

/*
Server loop. Creates Socket and contains loop for incoming connections, sending them
on a different thread for handling.
*/

const int PORT = 2040;
TCPSocket tcpSocket = new(); // Same as new TCPSocket()
DeviceManager deviceManager = new();

await tcpSocket.Create(PORT);
tcpSocket.Bind();
tcpSocket.Listen(15);


while (true)
{
    // Accept incoming connection
    Socket conn = await tcpSocket.Accept();

    // Sending connection to client handler in separate thread
    Thread clientHandler = new(() => deviceManager.HandleClient(conn, tcpSocket)); // lambda
    clientHandler.Start();
}

