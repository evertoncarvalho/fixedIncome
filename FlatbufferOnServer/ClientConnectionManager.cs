using System.Net;
using System.Net.Sockets;

namespace FlatbufferOnServer
{
    public abstract class ClientConnectionManager<MessageType, Message> : ServerConnectionManager<MessageType, Message>
    {
        protected ProtocolControl ProtocolControl { get; private set; }
        public override void Start(int port, string host = "127.0.0.1", int maxSimuntaneousRequestsBeforeBusy = 10)
        {
            try
            {
                IPAddress ipAddress = IPAddress.Parse(host);
                IPEndPoint endPoint = new IPEndPoint(ipAddress, port);
                Socket _socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _socket.Connect(endPoint);
                ProtocolControl = new(_socket);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
