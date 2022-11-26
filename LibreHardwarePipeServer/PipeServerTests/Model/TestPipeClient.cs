using System.IO.Pipes;
using System.Text;

namespace PipeServerTests.Model
{
    internal class TestPipeClient
    {
        private string _name;

        public TestPipeClient(string name)
        {
            _name = name;
        }

        public string SendRequest(string request)
        {
            var client = new NamedPipeClientStream("localhost", _name, PipeDirection.InOut, PipeOptions.Asynchronous);

            try
            {
                client.Connect(2000);
            }
            catch(Exception ex)
            {
                return ex.Message;
            }

            Send(client, request);
            return Receive(client);
        }

        private void Send(NamedPipeClientStream client, string data)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(data);

            client.Write(byteData, 0, byteData.Length);

            client.Flush();
        }

        private string Receive(NamedPipeClientStream client)
        {
            var data = new List<byte>();

            using (var keepAliveSource = new KeepAliveTokenSource(100))
            {
                keepAliveSource.TokenCancelled += (s, e) =>
                {
                    client.Close();
                };

                while (!keepAliveSource.IsCancelled)
                {
                    try
                    {
                        int b = client.ReadByte();

                        if (b == 0 || b == -1)
                        {
                            keepAliveSource.Kill();
                            break;
                        }

                        data.Add((byte)b);

                        keepAliveSource.KeepAlive();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            return Encoding.UTF8.GetString(data.ToArray());
        }
    }
}
