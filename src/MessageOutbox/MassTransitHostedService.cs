using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace MessageOutbox
{
    public class MassTransitHostedService : IHostedService
    {
        readonly IBusControl _bus;
        readonly ILogger _logger;

        public MassTransitHostedService(IBusControl bus, ILoggerFactory loggerFactory)
        {
            _bus = bus;
            _logger = loggerFactory.CreateLogger<MassTransitHostedService>();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting bus");
            await _bus.StartAsync(cancellationToken).ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping bus");
            return _bus.StopAsync(cancellationToken);
        }
    }
}
