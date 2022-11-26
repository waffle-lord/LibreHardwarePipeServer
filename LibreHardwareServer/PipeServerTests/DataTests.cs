using LibreHardwareServer;
using Newtonsoft.Json.Linq;
using PipeServerTests.Model;

namespace PipeServerTests
{
    [TestClass]
    public class DataTests
    {
        static HardwareServer server;

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

        private bool CheckStatus(string json)
        {
            try
            {
                if (json == null || json == "") return false;

                return JObject.Parse(json)["Status"].Value<int>() == 1;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }

        [TestMethod]
        public void GetCpuDataTest()
        {
            TestClient client = new TestClient();

            var data = client.SendRequest("cpu");

            Assert.IsTrue(CheckStatus(data));
            Console.WriteLine("Response is OK");
            Console.WriteLine($"RESPONSE:\n{data}");
        }

        [TestMethod]
        public void GetMemoryDataTest()
        {
            TestClient client = new TestClient();

            var data = client.SendRequest("memory");

            Assert.IsTrue(CheckStatus(data));
            Console.WriteLine("Response is OK");
            Console.WriteLine($"RESPONSE:\n{data}");

        }

        [TestMethod]
        public void GetGpuDataTest()
        {
            TestClient client = new TestClient();

            var data = client.SendRequest("gpu");

            Assert.IsTrue(CheckStatus(data));
            Console.WriteLine("Response is OK");
            Console.WriteLine($"RESPONSE:\n{data}");

        }

        [TestMethod]
        public void MultiRequestTest()
        {
            TestClient client = new TestClient();

            var data = client.SendRequest("cpu");

            Assert.IsTrue(CheckStatus(data));
            Console.WriteLine("Response 1 is OK");

            data = client.SendRequest("memory");
            Assert.IsTrue(CheckStatus(data));
            Console.WriteLine("Response 2 is OK");
        }

        [TestMethod]
        public void MultiConnectionTest()
        {
            List<Task> tasks = new List<Task>();
            List<string> responses = new List<string>();
            List<DelayedTestClient> clients = new List<DelayedTestClient>();

            for (int i = 0; i < 10; i++)
            {
                clients.Add(new DelayedTestClient());
            }

            foreach(var client in clients)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    responses.Add(client.SendRequest("cpu"));
                }));
            }

            Task.WaitAll(tasks.ToArray(), TimeSpan.FromSeconds(30));

            Console.WriteLine($"{responses.Count} responses recieved");

            Assert.IsTrue(responses.Count == 10);

            foreach (var response in responses)
            {
                Assert.IsTrue(CheckStatus(response));
            }

            Console.WriteLine("All Responses OK");
        }
    }
}