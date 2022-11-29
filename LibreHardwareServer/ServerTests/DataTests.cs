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
        [DataRow("cpu")]
        [DataRow("memory")]
        [DataRow("gpu")]
        [DataRow("klsjflksjdf", true)]
        public void GetData(string message, bool shouldFail = false)
        {
            TestClient client = new TestClient();

            var data = client.SendRequest(message);

            client.Close();

            Assert.IsTrue(shouldFail ? !CheckStatus(data) : CheckStatus(data));

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

            client.Close();

            Assert.IsTrue(CheckStatus(data));
            Console.WriteLine("Response 2 is OK");
        }

        [TestMethod]
        public void MultiConnectionTest()
        {
            List<Task> tasks = new List<Task>();
            List<string> responses = new List<string>();
            List<TestClient> clients = new List<TestClient>();

            for (int i = 0; i < 10; i++)
            {
                clients.Add(new TestClient());
            }

            foreach(var client in clients)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(2000);

                    responses.Add(client.SendRequest("cpu"));

                    client.Close();
                }));
            }

            Task.WaitAll(tasks.ToArray(), TimeSpan.FromSeconds(60));

            Console.WriteLine($"{responses.Count} responses recieved");

            Assert.IsTrue(responses.Count == 10);

            foreach (var response in responses)
            {
                Assert.IsTrue(CheckStatus(response));
            }

            Console.WriteLine("All Responses OK");
        }

        [TestMethod]
        public async Task TimeoutTest()
        {
            var client = new TestClient();

            var data = client.SendRequest("ping");

            Assert.IsTrue(CheckStatus(data));

            Console.WriteLine("Connected");

            await Task.Delay(10 * 1000);

            data = client.SendRequest("ping");

            Assert.IsTrue(CheckStatus(data));

            Console.WriteLine("10secs - Still Connected");

            await Task.Delay(50 * 1000);

            data = client.SendRequest("ping");

            Assert.IsTrue(CheckStatus(data));

            Console.WriteLine("50secs - Still Connected");

            await Task.Delay(70 * 1000);

            Console.WriteLine("1min, 10secs - Should throw exception (connection closed by server)");

            Assert.ThrowsException<IOException>(() => { client.SendRequest("ping"); });

            client.Close();
        }
    }
}