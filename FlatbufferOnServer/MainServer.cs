using System.Net;
using System.Net.Sockets;

namespace FlatbufferOnServer
{
    public abstract class MainServer <MessageType, Message>
    {
        protected delegate void MessageHandler(ProtocolControl clientSocket, Message message);

        protected List<ProtocolControl> Clients = new();
        protected Dictionary<MessageType, MessageHandler> Callbacks = new();
        public MainServer()
        {
        }
        public abstract void FillCallbacks();
        public virtual async void Start(int listenPort, int maxSimuntaneousRequestsBeforeBusy = 10)
        {
            IPAddress ipAddress = IPAddress.Any;
            IPEndPoint endPoint = new IPEndPoint(ipAddress, listenPort);
            Socket listener = new Socket(
                ipAddress.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            listener.Bind(endPoint);
            listener.Listen(maxSimuntaneousRequestsBeforeBusy);
            ProtocolControl socket;
            while (true)
            {
                Socket newClient = await listener.AcceptAsync();
                lock (Clients)
                {
                    socket = (ProtocolControl)Activator.CreateInstance(typeof(ProtocolControl), new object[] { newClient });
                    Clients.Add(socket);
                }
                OnNewClientConnection(socket);
            }
        }
        protected abstract Message GetMessageFromBytes(byte[] messageBytes);
        protected abstract bool IsValidMessage(Message message);
        protected abstract byte[] GetMessageBytes(MessageType type, Message message);
        protected abstract void ProcessMessage(ProtocolControl clientSocket, Message message);
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
        protected virtual byte[] PackMessage(MessageType type, Message message)
        {
            return ProtocolControl.AssemblyMessage(
                GetMessageBytes(type, message));
        }
    }
}
