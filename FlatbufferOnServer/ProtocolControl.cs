using System.Net.Sockets;

namespace FlatbufferOnServer
{
    internal delegate void Dispose(ProtocolControl sender);
    public class ProtocolControl
    {
        private DateTime _idleSince = DateTime.Now;
        private System.Timers.Timer _heartbeat = new System.Timers.Timer();
        internal Socket Handler { get; set; }
        internal event Dispose OnDispose;
        internal ProtocolControl(int heartbeatInSeconds = 30)
        {
            SetupHeartbeat(heartbeatInSeconds * 1000);
        }
        /// <summary>
        /// Build a buffer that contains a 4bytes header with length of the message and after that
        /// the message itself.
        /// </summary>
        /// <param name="bytes">Message in bytes created by the Flatbuffer message builder using function
        /// "ToSizedArray()" from DataBuffer.</param>
        /// <returns>4bytes header with length of the message followed by message itself</returns>
        internal static byte[] AssemblyMessage(byte[] bytes)
        {
            int offset = sizeof(int);
            byte[] buffer = new byte[offset + bytes.Length];
            BitConverter.GetBytes(bytes.Length).CopyTo(buffer, 0);
            bytes.CopyTo(buffer, offset);
            return buffer;
        }
        internal virtual byte[] Receive(ref byte[] data)
        {
            Handler.Receive(
                data,
                0,
                sizeof(int),
                SocketFlags.None);
            int size = BitConverter.ToInt32(data, 0);
            if (size > data.Length)
            {
                data = new byte[size];
            }
            Handler.Receive(
               data,
               0,
               size,
               SocketFlags.None);
            _idleSince = DateTime.Now;
            return data;
        }
        public virtual void Send(byte[] packMessage)
        {
            try
            {
                Handler.Send(packMessage);
                _idleSince = DateTime.Now;
            }
            catch (SocketException)
            {
                //Logger.Error($"fail to send package to {ClientData}. Disconnecting.");
                Disconnect();
            }
        }
        internal void Disconnect()
        {
            try
            {
                _heartbeat.Stop();
                _heartbeat.Dispose();
                Handler.Close();
                Handler.Dispose();
            }
            catch (Exception ex)
            {
                //Logger.Error(ex);
            }
            finally
            {
                OnDispose?.Invoke(this);
            }
        }
        private void SetupHeartbeat(double interval)
        {
            _heartbeat.Interval = interval;
            _heartbeat.Elapsed += Heartbeat_Elapsed;
            _heartbeat.Start();
        }
        private bool IdleExceeded
        {
            get
            {
                return (DateTime.Now - _idleSince).Seconds > _heartbeat.Interval / 1000;
            }
        }
        private void Heartbeat_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (IdleExceeded)
            {
                //Logger.Error($"connectionCheck: disconnecting {ClientData} due to idletime exceeded");
                Disconnect();
            }
        }
    }
}
