using System.Net.Sockets;
using System.Net;
using System.Text;

/*
This class is built around the Socket object.
It allows for easy initialization and implementation of Socket and it's methods.
*/
class TCPSocket
{
    private Socket? sock;
    private bool is_server;
    private IPEndPoint? deviceIp;

    private void Close()
    {
        sock?.Close();
    }

    // Constructor
    public TCPSocket()
    {
        this.is_server = false;
    }

    // Destructor
    ~TCPSocket()
    {
        this.Close();
    }

    public async Task Create(int port)
    {
        /* 
        Gets server (this device) IP addr and creates socket
        */
        // Gets IP
        string hostName = Dns.GetHostName();
        IPHostEntry ipList = await Dns.GetHostEntryAsync(hostName);
        IPAddress ipAddr = ipList.AddressList[1]; // 1 is for ipv4

        // Create endpoint
        this.deviceIp = new(ipAddr, port);

        // Create socket
        this.sock = new(
            deviceIp.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp
        );

        Console.WriteLine("Socket initialized...");
    }

    public void Bind()
    {
        /*
        Binds socket to IPEndpoint (ip addr/port)
        */
        if (this.sock == null || this.deviceIp == null)
        {
            throw new InvalidOperationException("Socket or IPEndpoint has not been created.");
        }
        this.sock.Bind(this.deviceIp);

        Console.WriteLine($"Socket binded to: {this.deviceIp}");
    }

    public void Listen(int backlog)
    /*
    Initiates listening state on socket with specified backlog
    */
    {
        if (this.sock == null)
        {
            throw new InvalidOperationException("Socket has not been created.");
        }
        this.sock.Listen(backlog);

        Console.WriteLine("Socket is now listening for incoming connections...");
    }

    public async Task<Socket> Accept()
    {
        /*
        Accepts incoming connection and returns socket for use.
        */
        if (this.sock == null)
        {
            throw new InvalidOperationException("Socket has not yet been created.");
        }

        Console.WriteLine("Socket waiting to accept connection...");
        Socket conn = await sock.AcceptAsync();
        Console.WriteLine("Socket accepted connection.");

        return conn; // replace
    }

    public async Task Connect(IPEndPoint ip)
    {
        /*
        Connects to IPEndpoint (ip addr/socket)
        */
        if (this.sock == null)
        {
            throw new InvalidOperationException("Socket has not yet been created.");
        }
        await this.sock.ConnectAsync(ip);

        Console.WriteLine($"Socket connecting to: {ip}");
    }

    public async Task Send(string message)
    {
        /*
        Converts message to bytes and sends without "awaiting" (underscore)
        */
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);

        if (this.sock == null)
        {
            throw new InvalidOperationException("Socket has not yet been created.");
        }
        Console.WriteLine("Sending message...");
        await this.sock.SendAsync(messageBytes, 0);
        Console.WriteLine("Message sent.");
    }

    public async Task SendLength(string message)
    {
        byte[] messageBytesLength = Encoding.UTF8.GetBytes(message);
        int length = messageBytesLength.Length;
        byte[] lengthPrefix = BitConverter.GetBytes(length);

        if (this.sock == null)
        {
            throw new InvalidOperationException("Socket has not yet been created.");
        }

        Console.WriteLine("Sending length...");
        _ = await this.sock.SendAsync(lengthPrefix, SocketFlags.None);
        Console.WriteLine("Length sent.");
    }

    public async Task<string> Receive(byte[] buffer, Socket conn)
    {
        /*
        Recieves bytes, converts to string, ensures bytes are complete and returns message.
        */
        Console.WriteLine("Receiving message from client...");

        int message = await conn.ReceiveAsync(buffer, SocketFlags.None);
        string messageString = Encoding.UTF8.GetString(buffer, 0, message);

        return messageString;
    }

    public async Task<int> ReceiveLength(byte[] buffer, Socket conn)
    {
        await conn.ReceiveAsync(buffer, SocketFlags.None);
        int messageLength = BitConverter.ToInt32(buffer);

        return messageLength;
    }
}