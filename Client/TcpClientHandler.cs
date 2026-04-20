using System.Net.Sockets;
using System.Text;

namespace Client
{
    public class TcpClientHandler
    {
        private readonly string _serverIp;
        private readonly int _port;

        public TcpClientHandler(string serverIp, int port)
        {
            _serverIp = serverIp;
            _port = port;
        }

        public async Task SendMessageAsync(string message)
        {
            using var client = new TcpClient();

            try
            {
                // Step 1: Connect to server
                await client.ConnectAsync(_serverIp, _port);
                Console.WriteLine($"Connected to server at {_serverIp}:{_port}");

                var stream = client.GetStream();
                var reader = new StreamReader(stream, Encoding.UTF8);
                var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                // Step 2: Encrypt and send message
                string encrypted = CryptoHelper.Encrypt(message);
                await writer.WriteLineAsync(encrypted);
                Console.WriteLine($"Sent (encrypted): {encrypted}");
                Console.WriteLine($"Sent (original):  {message}");
                Console.WriteLine("\n--- Server Response ---");

                // Step 3: Keep reading responses until server closes connection
                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    // Step 4: Decrypt and display each response
                    string decrypted = CryptoHelper.Decrypt(line);
                    Console.WriteLine(decrypted);
                }

                Console.WriteLine("--- End of Response ---");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}