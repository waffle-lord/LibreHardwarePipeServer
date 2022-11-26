using LibreHardwareServer.Handlers;
using LibreHardwareServer.Interfaces;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LibreHardwareServer
{
    public class HardwareServer
    {
        private bool _running;
        private IResponseHandler _responseHandler;
        private CancellationTokenSource _cancelSource;

        public HardwareServer(IResponseHandler handler = null)
        {
            _running = false;
            _cancelSource = new CancellationTokenSource();
            _responseHandler = handler ?? new DefaultResponseHandler();
        }

        public void Start()
        {
            _running = true;
            TcpListener tcplistener = new TcpListener(IPAddress.Parse("127.0.0.1"), 12345);
            tcplistener.Start();

            Task.Factory.StartNew(async () =>
            {
                while (_running)
                {
                    var client = await tcplistener.AcceptTcpClientAsync();

                    _ = Task.Factory.StartNew(() =>
                    {
                        HandleConnectionReceived(client);
                    });
                }
            }, _cancelSource.Token);
        }

        public void Stop()
        {
            _running = false;
            _cancelSource.Cancel();
        }

        private void HandleConnectionReceived(TcpClient client)
        {
            Stream stream = client.GetStream();

            List<byte> bytes = new List<byte>();
            int byteValue;

            while ((byteValue = stream.ReadByte()) != 1)
            {
                bytes.Add((byte)byteValue);
            }

            string message = Encoding.ASCII.GetString(bytes.ToArray(), 0, bytes.Count);

            var response = _responseHandler.HandleRequest(message);

            byte[] data = Encoding.ASCII.GetBytes(response + ((char)1));

            stream.Write(data, 0, data.Length);
            
            client.Close();
        }
    }
}