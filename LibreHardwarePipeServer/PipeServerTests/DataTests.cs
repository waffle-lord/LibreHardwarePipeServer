using LibreHardwarePipeServer;
using PipeServerTests.Model;

namespace PipeServerTests
{
    [TestClass]
    public class DataTests
    {
        static HardwareServer server;

        const string _name = "hwtesting";

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            server = new HardwareServer();

            server.Start();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            server.Stop();
        }

        [TestMethod]
        public void GetCpuDataTest()
        {
            TestClient client = new TestClient();

            var data = client.SendRequest("cpu");

            Console.WriteLine($"RESPONSE:\n{data}");

            Assert.IsTrue(data != null && data != "");
        }

        [TestMethod]
        public void MultiRequestTest()
        {
            TestClient client = new TestClient();

            var data = client.SendRequest("cpu");

            Assert.IsTrue(data != null && data != "");
            Console.WriteLine("Response 1 seems OK");

            data = client.SendRequest("memory");

            Assert.IsTrue(data != null && data != "");
            Console.WriteLine("Response 2 seems OK");
        }
    }
}