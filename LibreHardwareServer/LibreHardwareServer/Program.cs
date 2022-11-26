namespace LibreHardwareServer
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            HardwareServer server = new HardwareServer();

            Console.WriteLine("Starting Server ...");

            server.Start();

            Console.WriteLine("Waiting for connections ...");

            await Task.Delay(-1);
        }
    }
}