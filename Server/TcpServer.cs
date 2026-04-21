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
            try
            {
                _listener = new TcpListener(IPAddress.Any, _port);
                _listener.Start();
                Console.WriteLine($"Server started on port {_port}. Waiting for clients...");

                while (true)
                {
                    try
                    {
                        // Accept each client on a separate task (handles multiple clients)
                        var client = await _listener.AcceptTcpClientAsync();
                        Console.WriteLine("Client connected!");
                        _ = Task.Run(() => HandleClientAsync(client));
                    }
                    catch (ObjectDisposedException)
                    {
                        Console.WriteLine("Listener has been closed. Stopping server.");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error accepting client: {ex.Message}");
                    }
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Socket error: {ex.Message}. Port may already be in use.");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"Access denied: Port {_port} requires administrator privileges.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Server error: {ex.Message}");
            }
            finally
            {
                _listener?.Stop();
                _listener?.Dispose();
                Console.WriteLine("Server stopped.");
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            using (client)
            {
                try
                {
                    var stream = client.GetStream();
                    var reader = new StreamReader(stream, Encoding.UTF8);
                    var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                    try
                    {
                        // Step 1: Read encrypted message from client
                        string? encryptedInput = await reader.ReadLineAsync();
                        if (encryptedInput == null)
                        {
                            Console.WriteLine("Client sent no data.");
                            return;
                        }

                        // Step 2: Decrypt it
                        string input;
                        try
                        {
                            input = CryptoHelper.Decrypt(encryptedInput);
                            Console.WriteLine($"Received (decrypted): {input}");
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Error: Invalid encrypted format from client.");
                            string errorMsg = CryptoHelper.Encrypt("ERROR: Invalid format");
                            await writer.WriteLineAsync(errorMsg);
                            return;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Decryption error: {ex.Message}");
                            return;
                        }

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
                                try
                                {
                                    string timestamp = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                                    string encrypted = CryptoHelper.Encrypt(timestamp);
                                    await writer.WriteLineAsync(encrypted);
                                    Console.WriteLine($"Sent: {timestamp}");

                                    if (i < n - 1)
                                        await Task.Delay(1000); // 1 second gap between sends
                                }
                                catch (IOException ex)
                                {
                                    Console.WriteLine($"Network error while sending: {ex.Message}");
                                    break;
                                }
                            }
                        }
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine($"Stream error: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Unexpected error handling client: {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting client stream: {ex.Message}");
                }
            }

            Console.WriteLine("Client disconnected.");
        }
    }
}