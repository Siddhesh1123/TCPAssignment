using System.Net.Sockets;
using System.Text;
using Client.Domain.Interfaces;
using Client.Domain.Entities;

namespace Client.Infrastructure.Networking
{
    public class ClientConnectionHandler
    {
        private readonly string _serverIp;
        private readonly int _port;
        private readonly ICryptoService _cryptoService;

        public ClientConnectionHandler(string serverIp, int port, ICryptoService cryptoService)
        {
            _serverIp = serverIp;
            _port = port;
            _cryptoService = cryptoService;
        }

        public async Task<ServerResponse> SendMessageAsync(string message)
        {
            var response = new ServerResponse();

            using var client = new TcpClient();

            try
            {
                using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(5));
                await client.ConnectAsync(_serverIp, _port, cts.Token);
                Console.WriteLine($"Connected to server at {_serverIp}:{_port}");

                using var stream = client.GetStream();
                var reader = new StreamReader(stream, Encoding.UTF8);
                var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                try
                {
                    string encrypted = _cryptoService.Encrypt(message);
                    await writer.WriteLineAsync(encrypted);
                    Console.WriteLine($"Sent (encrypted): {encrypted}");
                    Console.WriteLine($"Sent (original):  {message}");
                    Console.WriteLine("\n--- Server Response ---");

                    string? line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        try
                        {
                            string decrypted = _cryptoService.Decrypt(line);
                            Console.WriteLine(decrypted);
                            response.Messages.Add(decrypted);
                        }
                        catch (InvalidOperationException ex)
                        {
                            Console.WriteLine($"Error decrypting response: {ex.Message}");
                            response.ErrorMessage = ex.Message;
                            break;
                        }
                    }

                    Console.WriteLine("--- End of Response ---");
                    response.IsSuccessful = true;
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Network error during communication: {ex.Message}");
                    response.IsSuccessful = false;
                    response.ErrorMessage = ex.Message;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during message exchange: {ex.Message}");
                    response.IsSuccessful = false;
                    response.ErrorMessage = ex.Message;
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Connection timeout: Server at {_serverIp}:{_port} did not respond within 5 seconds.");
                response.IsSuccessful = false;
                response.ErrorMessage = "Connection timeout";
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}. Check if server is running at {_serverIp}:{_port}");
                response.IsSuccessful = false;
                response.ErrorMessage = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Encryption error: {ex.Message}");
                response.IsSuccessful = false;
                response.ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                response.IsSuccessful = false;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }
    }
}
