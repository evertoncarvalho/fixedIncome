using System.Net;
using System.Net.Sockets;

namespace FlatbufferOnServer
{
    public abstract class ServerConnectionManager <MessageType, Message>
    {
        protected delegate void MessageHandler(ProtocolControl clientSocket, Message message);

        protected List<ProtocolControl> Clients = new();
        protected Dictionary<MessageType, MessageHandler> Callbacks = new();
        public ServerConnectionManager()
        {
            FillCallbacks();
        }
        protected abstract void FillCallbacks();
        protected abstract Message GetMessageFromBytes(byte[] messageBytes);
        protected abstract bool IsValidMessage(Message message);
        protected abstract byte[] GetMessageBytes(MessageType type, object message);
        protected abstract void ProcessMessage(ProtocolControl clientSocket, Message message);
        public virtual async void Start(int port, string host = "",
            int maxSimuntaneousRequestsBeforeBusy = 10)
        {
            IPAddress ipAddress = IPAddress.Any;
            IPEndPoint endPoint = new IPEndPoint(ipAddress, port);
            Socket listener = new Socket(
                ipAddress.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            listener.Bind(endPoint);
            listener.Listen(maxSimuntaneousRequestsBeforeBusy);
            ProtocolControl protocolControl;
            while (true)
            {
                Socket newClient = await listener.AcceptAsync();
                lock (Clients)
                {
                    protocolControl = new (newClient);
                    Clients.Add(protocolControl);
                }
                OnNewClientConnection(protocolControl);
            }
        }
        protected virtual async void OnNewClientConnection(ProtocolControl socket)
        {
            byte[] bytes = new byte[8];
            while (true)
            {
                try
                {
                    var message = GetMessageFromBytes(
                        socket.Receive(ref bytes));
                    if (IsValidMessage(message))
                    {
                        ProcessMessage(socket, message);
                    }
                    else
                    {
                        //Logger.Debug($"invalid message received {message.MessageType} on {ServiceName}");
                    }
                }
                catch (SocketException)
                {
                    //disconnection;
                    //Logger.Error($"remote disconnection from {socket.ClientData} on {ServiceName}");
                    socket.Disconnect();
                    break;
                }
                catch
                {
                    //Logger.Error($"invalid message received {bytes.Length} bytes. disconnected on {ServiceName}.");
                    socket.Disconnect();
                    break;
                }
            }
        }
        protected virtual byte[] PackMessage(MessageType type, object message)
        {
            return ProtocolControl.AssemblyMessage(
                GetMessageBytes(type, message));
        }
    }
}
