using Server.Domain.Interfaces;
using Server.Infrastructure.Networking;

namespace Server.Application.Services
{
    public class ServerService
    {
        private readonly ServerConnectionHandler _connectionHandler;

        public ServerService(int port, ICryptoService cryptoService, IDataRepository dataRepository)
        {
            _connectionHandler = new ServerConnectionHandler(port, cryptoService, dataRepository);
        }

        public async Task RunAsync()
        {
            await _connectionHandler.StartAsync();
        }
    }
}
