using LibreHardwarePipeServer.Handlers;
using LibreHardwarePipeServer.Interfaces;
using System.IO.Pipes;
using System.Text;

namespace LibreHardwarePipeServer
{

    public class HardwarePipeServer
    {
        private string _name;

        private bool _running = false;

        private CancellationTokenSource _cancelSource = new CancellationTokenSource();

        private IResponseHandler _responseHanlder;

        public HardwarePipeServer(string name, IResponseHandler? handler = null)
        {
            _responseHanlder = handler ?? new DefaultResponseHandler();

            _name = name;
        }

        private async Task HandleConnection()
        {
            var pipeStream = new NamedPipeServerStream(_name, PipeDirection.InOut, 1, PipeTransmissionMode.Byte);

            await pipeStream.WaitForConnectionAsync();

            _ = Task.Factory.StartNew(async() =>
            {
                await HandleConnection();
            });

            while (pipeStream.IsConnected)
            {
                await ReadDataAsync(pipeStream);
            }

            pipeStream.Close();
        }

        public void Start()
        {
            _running = true;

            Task.Factory.StartNew(async() =>
            {
                while (_running)
                {
                    await HandleConnection();
                }
            }, _cancelSource.Token);
        }

        public void StopAsync()
        {
            _running = false;

            _cancelSource.Cancel();
        }

        private async Task ReadDataAsync(NamedPipeServerStream pipeStream)
        {
            try
            {
                byte[] buffer = new byte[10]; // TODO - re-write this

                var len = pipeStream.Read(buffer, 0, buffer.Length);

                var dataString = Encoding.UTF8.GetString(buffer[0..len]);

                var responseData = _responseHanlder.HandleRequest(dataString);

                await WriteDataAsync(responseData, pipeStream);

                return;
            }
            catch (Exception ex)
            {
                // TODO - logging?
            }
        }

        private async Task WriteDataAsync(string data, NamedPipeServerStream pipeStream) //, StreamWriter sw)
        {
            try
            {
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);

                await pipeStream.WriteAsync(dataBytes, 0, dataBytes.Length);

                await pipeStream.FlushAsync();

                return;
            }
            catch (Exception ex)
            {
                // TODO - logging?
            }
        }
    }
}
