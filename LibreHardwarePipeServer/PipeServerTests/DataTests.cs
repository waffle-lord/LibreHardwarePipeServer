using LibreHardwarePipeServer;
using PipeServerTests.Model;

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

            server.Start();
        }

        [TestMethod]
        public void GetCpuDataTest()
        {
            string data = "";

            try
            {
                var client = new TestPipeClient(_name);

                data = client.SendRequest("cpu");

                Assert.IsTrue(data != null && data != "");

                Console.WriteLine(data);
            }
            catch
            {
                Console.WriteLine("disconnect");
                Assert.Fail();
            }
        }

        [TestMethod]
        public void MultiRequestTest()
        {
            string data = "";

            try
            {
                var client = new TestPipeClient(_name);

                data = client.SendRequest("cpu");

                Assert.IsTrue(data != null && data != "");

                Console.WriteLine("Request 1 Has Data");

                data = "";

                data = client.SendRequest("cpu");

                Assert.IsTrue(data != null && data != "");

                Console.WriteLine("Request 2 Has Data");
            }
            catch
            {
                Console.WriteLine("disconnected");
                Assert.Fail();
            }
        }
    }
}