using LibreHardwareServer.Handlers;
using LibreHardwareServer.Interfaces;
using LibreHardwareServer.Model;
using System.Net;
using System.Net.Sockets;

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

        private System.Timers.Timer _keepAliveTimer = new System.Timers.Timer(TimeSpan.FromSeconds(3).TotalMilliseconds);

        int maxConnections = 20;
        private List<TcpConnection> _connections = new List<TcpConnection>();

        public HardwareServer(IResponseHandler handler = null)
        {
            _cancelSource = new CancellationTokenSource();
            _responseHandler = handler ?? new DefaultResponseHandler();

            _keepAliveTimer.Elapsed += _keepAliveTimer_Elapsed;

            _keepAliveTimer.Start();
        }

        private void _keepAliveTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            var closableConnections = _connections.Where(x => x.LastResponseTime.AddMinutes(1) < DateTime.Now).ToList();

            foreach(var connection in closableConnections)
            {
                RemoveConnection(connection);
            }
        }

        private Func<int, string> clientTag = (id) => $"Conn#{id}";

        private void Output(string tag, string message)
        {
            Console.WriteLine($"[{tag}]: {message}");
        }

        private int AddConnection(TcpClient client)
        {
            for (int i = 0; i < maxConnections; i++)
            {
                if (!_connections.Any(x => x.Id == i))
                {
                    _connections.Add(new TcpConnection(i, client));
                    Output(clientTag(i), "Connected");
                    return i;
                }
            }

            client.Close();
            return -1;
        }

        private void RemoveConnection(TcpConnection connection)
        {
            connection.Close();

            if(_connections.Contains(connection))
            {
                _connections.Remove(connection);
                Output(clientTag(connection.Id), "Closed");
            }
        }

        public void Start()
        {
            if (_running) return;

            Output("Server", "Starting up ...");

            _running = true;
            TcpListener tcplistener = new TcpListener(_address, _port);
            tcplistener.Start();

            Task.Factory.StartNew(async () =>
            {
                while (_running)
                {
                    if (_connections.Count >= maxConnections)
                    {
                        if (!_notifiedMaxConnected)
                        {
                            _notifiedMaxConnected = true;
                            Output("Server", $"-- Max connections Reached --\n >> Connections: {_connections.Count}/{maxConnections}");
                        }

                        continue;
                    }

                    int clientId = AddConnection(await tcplistener.AcceptTcpClientAsync());

                    if (clientId == -1)
                    {
                        Output("Server", $"Failed to add client\n >> Connections: {_connections.Count}/{maxConnections}");
                        continue;
                    }

                    _notifiedMaxConnected = false;

                    _ = Task.Factory.StartNew(() =>
                    {
                        HandleConnectionReceived(clientId);
                    });
                }
            }, _cancelSource.Token);

            Output("Server", "Started. Waiting for connections");
        }

        public void Stop()
        {
            Output("Server", "Shutting down ...");

            _running = false;
            _cancelSource.Cancel();
            
            foreach(var connection in _connections)
            {
                RemoveConnection(connection);
            }

            Output("Server", "Server Stopped");
        }

        private void HandleConnectionReceived(int clientId)
        {
            var connection = _connections.FirstOrDefault(x => x.Id == clientId);

            if (connection == null)
            {
                Output("Server", $"Failed to find client with ID: {clientId}");
                return;
            }

            while (connection.IsConnected)
            {
                var message = connection.Receive();

                Output(clientTag(clientId), $"<- Recv: {message}");

                var response = _responseHandler.HandleRequest(message);

                connection.Send(response.Serialize());

                Output(clientTag(clientId), $"-> Responded");

                if (response.Status == ResponseStatus.Closing)
                {
                    connection.Close();
                }
            }

            RemoveConnection(connection);
        }
    }
}