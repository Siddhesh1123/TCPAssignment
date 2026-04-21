using Client.Domain.Interfaces;
using Client.Infrastructure.Networking;

namespace Client.Application.Services
{
    public class ClientService
    {
        private readonly ClientConnectionHandler _connectionHandler;

        public ClientService(string serverIp, int port, ICryptoService cryptoService)
        {
            _connectionHandler = new ClientConnectionHandler(serverIp, port, cryptoService);
        }

        public async Task SendAsync(string message)
        {
            await _connectionHandler.SendMessageAsync(message);
        }
    }
}
