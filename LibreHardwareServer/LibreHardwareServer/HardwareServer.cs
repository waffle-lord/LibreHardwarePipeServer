using LibreHardwareServer.Handlers;
using LibreHardwareServer.Interfaces;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LibreHardwareServer
{
    public class HardwareServer
    {
        private IPAddress _address = IPAddress.Parse("127.0.0.1");
        private int _port = 22528;
        private bool _running = false;
        private IResponseHandler _responseHandler;
        private CancellationTokenSource _cancelSource;
        private bool _notifiedMaxConnected = false;

        int maxConnections = 20;
        Dictionary<int, TcpClient> connections = new Dictionary<int, TcpClient>();

        public HardwareServer(IResponseHandler handler = null)
        {
            _cancelSource = new CancellationTokenSource();
            _responseHandler = handler ?? new DefaultResponseHandler();
        }

        private Func<int, string> clientTag = (id) => $"Conn#{id}";

        private void Output(string tag, string message)
        {
            Console.WriteLine($"[{tag}]: {message}");
        }

        private int AddClient(TcpClient client)
        {
            for (int i = 0; i < maxConnections; i++)
            {
                if (!connections.ContainsKey(i))
                {
                    Output(clientTag(i), "Connected");
                    connections.Add(i, client);
                    return i;
                }
            }

            client.Close();
            return -1;
        }

        private void RemoveClient(int clientId)
        {
            if(connections.ContainsKey(clientId))
            {
                connections.Remove(clientId);
                Output(clientTag(clientId), "Closed");
            }
        }

        public void Start()
        {
            _running = true;
            TcpListener tcplistener = new TcpListener(_address, _port);
            tcplistener.Start();

            Task.Factory.StartNew(async () =>
            {
                while (_running)
                {
                    if (connections.Count >= maxConnections)
                    {
                        if (!_notifiedMaxConnected)
                        {
                            _notifiedMaxConnected = true;
                            Output("Server", $"-- Max connections Reached --\n >> Connections: {connections.Count}/{maxConnections}");
                        }

                        continue;
                    }

                    int clientId = AddClient(await tcplistener.AcceptTcpClientAsync());

                    if (clientId == -1)
                    {
                        Output("Server", $"Failed to add client\n >> Connections: {connections.Count}/{maxConnections}");
                        continue;
                    }

                    _notifiedMaxConnected = false;

                    _ = Task.Factory.StartNew(() =>
                    {
                        HandleConnectionReceived(clientId);
                    });
                }
            }, _cancelSource.Token);
        }

        public void Stop()
        {
            _running = false;
            _cancelSource.Cancel();
        }

        private void HandleConnectionReceived(int clientId)
        {
            TcpClient client;

            if (connections.ContainsKey(clientId))
            {
                client = connections[clientId];
            }
            else
            {
                Output("Server", $"Failed to find client with ID: {clientId}");
                return;
            }

            Stream stream = client.GetStream();

            List<byte> bytes = new List<byte>();
            int byteValue;

            while ((byteValue = stream.ReadByte()) != 1)
            {
                bytes.Add((byte)byteValue);
            }

            string message = Encoding.ASCII.GetString(bytes.ToArray(), 0, bytes.Count);

            Output(clientTag(clientId), $"<- Recv: {message}");

            var response = _responseHandler.HandleRequest(message);

            byte[] data = Encoding.ASCII.GetBytes(response + ((char)1));

            stream.Write(data, 0, data.Length);

            Output(clientTag(clientId), $"-> Responded");

            client.Close();

            RemoveClient(clientId);
        }
    }
}