using System.Net.Sockets;
using System.Text;

namespace PipeServerTests.Model
{
    internal class TestClient
    {
        protected int _port = 22528;
        protected string _address = "127.0.0.1";

        public virtual string SendRequest(string message)
        {
            TcpClient client = new TcpClient(_address, _port);
            return Send(client, message);
        }

        protected string Send(TcpClient client, string message)
        {
            Stream stream = client.GetStream();

            byte[] data = Encoding.ASCII.GetBytes(message.ToString() + ((char)1));
            stream.Write(data, 0, data.Length);

            List<byte> bytes = new List<byte>();
            int byteValue;

            while ((byteValue = stream.ReadByte()) != 1)
            {
                bytes.Add((byte)byteValue);
            }

            var response = Encoding.ASCII.GetString(bytes.ToArray(), 0, bytes.Count);

            client.Close();

            return response;
        }
    }
}