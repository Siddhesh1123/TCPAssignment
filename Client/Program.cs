namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Write("Enter server IP (e.g. 127.0.0.1): ");
            string? ip = Console.ReadLine();

            Console.Write("Enter server port (e.g. 5000): ");
            string? portInput = Console.ReadLine();

            if (!int.TryParse(portInput, out int port))
            {
                Console.WriteLine("Invalid port. Exiting.");
                return;
            }

            Console.Write("Enter message (e.g. SetA-Two): ");
            string? message = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(ip) || string.IsNullOrWhiteSpace(message))
            {
                Console.WriteLine("Invalid input. Exiting.");
                return;
            }

            var clientHandler = new TcpClientHandler(ip, port);
            await clientHandler.SendMessageAsync(message);

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}