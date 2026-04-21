using Client.Application.Services;
using Client.Infrastructure.Crypto;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.Write("Enter server IP (e.g. 127.0.0.1): ");
                string? ip = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(ip))
                {
                    Console.WriteLine("Invalid IP address. Exiting.");
                    return;
                }

                Console.Write("Enter server port (e.g. 5000): ");
                string? portInput = Console.ReadLine();

                if (!int.TryParse(portInput, out int port) || port <= 0 || port > 65535)
                {
                    Console.WriteLine("Invalid port. Port must be between 1 and 65535. Exiting.");
                    return;
                }

                Console.Write("Enter message (e.g. SetA-Two): ");
                string? message = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(message))
                {
                    Console.WriteLine("Invalid message. Exiting.");
                    return;
                }

                // Dependency Injection
                var cryptoService = new CryptoService();
                var clientService = new ClientService(ip, port, cryptoService);

                await clientService.SendAsync(message);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation was cancelled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
        }
    }
}