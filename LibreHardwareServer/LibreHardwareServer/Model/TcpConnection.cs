using System.Net.Sockets;
using System.Text;

namespace LibreHardwareServer.Model
{
    internal class TcpConnection
    {
        public int Id { get; private set; }
        private TcpClient _client;
        public DateTime LastResponseTime { get; private set; }
        public bool IsConnected { get; private set; }
        private Stream _ioStream;

        public TcpConnection(int id, TcpClient client)
        {
            Id = id;
            _client = client;
            _ioStream = _client.GetStream();
            IsConnected = true;
            LastResponseTime = DateTime.Now;
        }

        public void Send(string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message + ((char)1));

            _ioStream.Write(data, 0, data.Length);

            LastResponseTime = DateTime.Now;
        }

        public string Receive()
        {
            List<byte> bytes = new List<byte>();
            int byteValue;

            while ((byteValue = _ioStream.ReadByte()) != 1)
            {
                bytes.Add((byte)byteValue);
            }

            return Encoding.ASCII.GetString(bytes.ToArray(), 0, bytes.Count);
        }

        public void Close()
        {
            IsConnected = false;
            _ioStream.Close();
            _client.Close();
        }
    }
}
