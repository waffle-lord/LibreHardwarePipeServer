namespace LibreHardwarePipeServer
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            string name = "librehwpipe";

            if (args.Length == 2 && args[0].ToLower() == "--name")
            {
                name = args[1];
            }

            var server = new HardwarePipeServer(name);

            server.Start();

            await Task.Delay(-1);
        }
    }
}