using Server.Application.Services;
using Server.Infrastructure.Crypto;
using Server.Infrastructure.Data;

namespace Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.Write("Enter port number to listen on (e.g. 5000): ");
                string? portInput = Console.ReadLine();

                if (!int.TryParse(portInput, out int port) || port <= 0 || port > 65535)
                {
                    Console.WriteLine("Invalid port. Port must be between 1 and 65535. Exiting.");
                    return;
                }

                // Dependency Injection
                var cryptoService = new CryptoService();
                var dataRepository = new DataRepository();
                var serverService = new ServerService(port, cryptoService, dataRepository);

                await serverService.RunAsync();
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Server operation was cancelled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error: {ex.Message}");
            }
        }
    }
}