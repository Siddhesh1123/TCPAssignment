using System;
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
                // Step 1: Connect to server with timeout
                using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(5));
                await client.ConnectAsync(_serverIp, _port, cts.Token);
                Console.WriteLine($"Connected to server at {_serverIp}:{_port}"); // Connection successful Server and Client are connected and ready to exchange messages

                using var stream = client.GetStream();
                var reader = new StreamReader(stream, Encoding.UTF8);
                var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                try
                {
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
                        try
                        {
                            // Step 4: Decrypt and display each response
                            string decrypted = CryptoHelper.Decrypt(line);
                            Console.WriteLine(decrypted);
                        }
                        catch (InvalidOperationException ex)
                        {
                            Console.WriteLine($"Error decrypting response: {ex.Message}");
                            break;
                        }
                    }

                    Console.WriteLine("--- End of Response ---");
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Network error during communication: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during message exchange: {ex.Message}");
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Connection timeout: Server at {_serverIp}:{_port} did not respond within 5 seconds.");
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}. Check if server is running at {_serverIp}:{_port}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Encryption error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}