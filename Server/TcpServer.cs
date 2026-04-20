using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class TcpServer
    {
        private readonly int _port;
        private TcpListener? _listener;

        public TcpServer(int port)
        {
            _port = port;
        }

        public async Task StartAsync()
        {
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            Console.WriteLine($"Server started on port {_port}. Waiting for clients...");

            while (true)
            {
                // Accept each client on a separate task (handles multiple clients)
                var client = await _listener.AcceptTcpClientAsync();
                Console.WriteLine("Client connected!");
                _ = Task.Run(() => HandleClientAsync(client));
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            using (client)
            {
                var stream = client.GetStream();
                var reader = new StreamReader(stream, Encoding.UTF8);
                var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                try
                {
                    // Step 1: Read encrypted message from client
                    string? encryptedInput = await reader.ReadLineAsync();
                    if (encryptedInput == null) return;

                    // Step 2: Decrypt it
                    string input = CryptoHelper.Decrypt(encryptedInput);
                    Console.WriteLine($"Received (decrypted): {input}");

                    // Step 3: Lookup in DataStore
                    int? result = DataStore.Lookup(input);

                    if (result == null)
                    {
                        // Not found — send EMPTY
                        string encryptedEmpty = CryptoHelper.Encrypt("EMPTY");
                        await writer.WriteLineAsync(encryptedEmpty);
                        Console.WriteLine("Sent: EMPTY");
                    }
                    else
                    {
                        int n = result.Value;
                        Console.WriteLine($"Sending timestamp {n} times...");

                        // Step 4: Send current timestamp n times with 1 second interval
                        for (int i = 0; i < n; i++)
                        {
                            string timestamp = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                            string encrypted = CryptoHelper.Encrypt(timestamp);
                            await writer.WriteLineAsync(encrypted);
                            Console.WriteLine($"Sent: {timestamp}");

                            if (i < n - 1)
                                await Task.Delay(1000); // 1 second gap between sends
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error handling client: {ex.Message}");
                }
            }

            Console.WriteLine("Client disconnected.");
        }
    }
}