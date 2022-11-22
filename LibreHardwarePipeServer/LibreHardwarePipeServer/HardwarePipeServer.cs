using LibreHardwarePipeServer.Handlers;
using LibreHardwarePipeServer.Interfaces;
using System.Globalization;
using System.IO.Pipes;
using System.Text;
using System.Text.Unicode;

namespace LibreHardwarePipeServer
{
    
    public class HardwarePipeServer
    {
        private NamedPipeServerStream pipeStream;

        IResponseHandler _responseHanlder;

        public HardwarePipeServer(string name, IResponseHandler? handler = null)
        {
            _responseHanlder = handler ?? new DefaultResponseHandler();

            pipeStream = new NamedPipeServerStream(name, PipeDirection.InOut, 1, PipeTransmissionMode.Byte);
        }

        public async Task RunAsync()
        {
            await pipeStream.WaitForConnectionAsync();

            while (true)
            {
                await ReadDataAsync();
            }
        }

        public async Task ReadDataAsync()
        {
            try
            {
                int read = -2;
                int index = 0;
                byte[] buffer = new byte[10];

                while (index < buffer.Length || read == 0 || read == -1) // I hate buffers, this is WIP -waffle
                {
                    read = pipeStream.ReadByte();
                    buffer[index++] = (byte)read;
                }

                var dataString = UTF8Encoding.UTF8.GetString(buffer);

                var responseData = _responseHanlder.HandleRequest(dataString);

                await WriteDataAsync(responseData);

                return;
            }
            catch (Exception ex)
            {
                // TODO - logging?
            }
        }

        public async Task WriteDataAsync(string data)
        {
            using var sw = new StreamWriter(pipeStream);

            try
            {
                await sw.WriteAsync(data);
                sw.Flush();

                return;
            }
            catch (Exception ex)
            {
                // TODO - logging?
            }
        }
    }
}
