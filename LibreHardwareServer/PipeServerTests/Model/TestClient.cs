using System.Net.Sockets;
using System.Text;

namespace PipeServerTests.Model
{
    internal class TestClient : IDisposable
    {
        protected int _port = 22528;
        protected string _address = "127.0.0.1";
        private TcpClient _client;
        private Stream _ioStream;

        public TestClient()
        {
            _client = new TcpClient(_address, _port);
            _ioStream = _client.GetStream();
        }

        public string SendRequest(string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message.ToString() + ((char)1));
            _ioStream.Write(data, 0, data.Length);

            List<byte> bytes = new List<byte>();
            int byteValue;

            while ((byteValue = _ioStream.ReadByte()) != 1)
            {
                bytes.Add((byte)byteValue);
            }

            var response = Encoding.ASCII.GetString(bytes.ToArray(), 0, bytes.Count);

            return response;
        }

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            try
            {
                SendRequest("close");
            }
            catch { } // connection is most likely closed if this throws, just continue

            _ioStream.Close();
            _client.Close();
        }
    }
}