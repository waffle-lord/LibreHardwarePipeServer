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

            server.Start();

            await Task.Delay(-1);
        }
    }
}