using System.Net.Sockets;

namespace PipeServerTests.Model
{
    internal class DelayedTestClient : TestClient
    {
        public override string SendRequest(string message)
        {
            TcpClient client = new TcpClient(_address, _port);

            Task.Delay(2000).GetAwaiter().GetResult();

            return Send(client, message);
        }
    }
}
