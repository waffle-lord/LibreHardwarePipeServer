using LibreHardwarePipeServer;
using PipeServerTests.Model;
using System.IO.Pipes;

namespace PipeServerTests
{
    [TestClass]
    public class DataTests
    {
        static HardwarePipeServer server;

        const string _name = "hwtesting";

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            server = new HardwarePipeServer(_name);

            server.StartAsync();
        }

        [TestMethod]
        public void GetCpuDataTest()
        {
            string data = "";

            try
            {
                using (var client = new TestPipeClient(_name))
                {
                    data = client.SendRequest("cpu");

                    Assert.IsTrue(data != null && data != "");

                    Console.WriteLine(data);
                }
            }
            catch
            {
                Console.WriteLine("disconnect");
            }

            Console.WriteLine(data);
        }

        [TestMethod]
        public void MultiRequestTest()
        {
            string data = "";

            try
            {
                using (var client = new TestPipeClient(_name))
                {
                    data = client.SendRequest("cpu");

                    Assert.IsTrue(data != null && data != "");

                    data = "";

                    client.SendRequest("cpu");

                    Assert.IsTrue(data != null && data != "");
                }
            }
            catch
            {
                Console.WriteLine("disconnected");
            }
        }
    }
}