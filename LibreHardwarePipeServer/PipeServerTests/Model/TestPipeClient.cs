using System.IO.Pipes;
using System.Text;

namespace PipeServerTests.Model
{
    internal class TestPipeClient : IDisposable
    {
        private NamedPipeClientStream _client;
        public TestPipeClient(string name)
        {
            _client = new NamedPipeClientStream(name);

            _client.Connect();
        }

        public string SendRequest(string request)
        {
            Send(request);
            return Receive();
        }

        private void Send(string data)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(data);

            _client.Write(byteData, 0, byteData.Length);

            _client.Flush();
        }

        private string Receive()
        {
            throw new NotImplementedException();

            byte[] data = new byte[] { };

            var buffer = new byte[1024];

            while () // TODO - finish this lol -waffle
            {
                var len = _client.Read(buffer, 0, buffer.Length);
            }

            //return Encoding.UTF8.GetString(buffer[0..len]);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
