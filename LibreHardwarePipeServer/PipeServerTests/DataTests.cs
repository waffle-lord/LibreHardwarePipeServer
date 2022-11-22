using LibreHardwarePipeServer;
using System.IO.Pipes;

namespace PipeServerTests
{
    [TestClass]
    public class DataTests
    {
        static HardwarePipeServer server;
        static NamedPipeClientStream client;

        const string _name = "hwtesting";

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            server = new HardwarePipeServer(_name);
            client = new NamedPipeClientStream(_name);

            Task.Run(async() =>
            {
                await server.RunAsync();
            });

            client.Connect();
            client.ReadMode = PipeTransmissionMode.Byte;
        }

        [TestMethod]
        public void GetCpuDataTest()
        {
            using var sw = new StreamWriter(client);
            using var sr = new StreamReader(client);

            sw.Write("cpu");
            sw.Flush();

            var data = sr.ReadToEnd();

            Console.WriteLine(data);
        }
    }
}