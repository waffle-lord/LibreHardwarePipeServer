using System.Net.Sockets;

namespace PipeServerTests.Model
{
    internal class DelayedTestClient : TestClient
    {
        private TimeSpan _delay;
        public DelayedTestClient(TimeSpan delay)
        {
            _delay = delay;
        }

        public override string SendRequest(string message)
        {
            TcpClient client = new TcpClient(_address, _port);

            Task.Delay(_delay).GetAwaiter().GetResult();

            return Send(client, message);
        }
    }
}
