using System.Net;
using System.Net.Sockets;
using System.Text;
using Server.Domain.Interfaces;

namespace Server.Infrastructure.Networking
{
    public class ServerConnectionHandler
    {
        private readonly int _port;
        private TcpListener? _listener;
        private readonly ICryptoService _cryptoService;
        private readonly IDataRepository _dataRepository;

        public ServerConnectionHandler(int port, ICryptoService cryptoService, IDataRepository dataRepository)
        {
            _port = port;
            _cryptoService = cryptoService;
            _dataRepository = dataRepository;
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
                        string? encryptedInput = await reader.ReadLineAsync();
                        if (encryptedInput == null)
                        {
                            Console.WriteLine("Client sent no data.");
                            return;
                        }

                        string input;
                        try
                        {
                            input = _cryptoService.Decrypt(encryptedInput);
                            Console.WriteLine($"Received (decrypted): {input}");
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Error: Invalid encrypted format from client.");
                            string errorMsg = _cryptoService.Encrypt("ERROR: Invalid format");
                            await writer.WriteLineAsync(errorMsg);
                            return;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Decryption error: {ex.Message}");
                            return;
                        }

                        int? result = _dataRepository.Lookup(input);

                        if (result == null)
                        {
                            string encryptedEmpty = _cryptoService.Encrypt("EMPTY");
                            await writer.WriteLineAsync(encryptedEmpty);
                            Console.WriteLine("Sent: EMPTY");
                        }
                        else
                        {
                            int n = result.Value;
                            Console.WriteLine($"Sending timestamp {n} times...");

                            for (int i = 0; i < n; i++)
                            {
                                try
                                {
                                    string timestamp = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                                    string encrypted = _cryptoService.Encrypt(timestamp);
                                    await writer.WriteLineAsync(encrypted);
                                    Console.WriteLine($"Sent: {timestamp}");

                                    if (i < n - 1)
                                        await Task.Delay(1000);
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
