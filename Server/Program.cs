namespace Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Write("Enter port number to listen on (e.g. 5000): ");
            string? portInput = Console.ReadLine();

            if (!int.TryParse(portInput, out int port))
            {
                Console.WriteLine("Invalid port. Exiting.");
                return;
            }

            var server = new TcpServer(port);
            await server.StartAsync();
        }
    }
}