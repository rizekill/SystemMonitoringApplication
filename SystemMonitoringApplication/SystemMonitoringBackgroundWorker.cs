using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using SM.Domain.Interfaces;

namespace SystemMonitoringApplication
{
    public class SystemMonitoringBackgroundWorker : IHostedService
    {
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly IMonitorService _service;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public SystemMonitoringBackgroundWorker(IMonitorService service, IHostApplicationLifetime appLifetime)
        {
            _service = service;
            _appLifetime = appLifetime;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(OnStarted);
            _appLifetime.ApplicationStopping.Register(OnStopping);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;

        private void OnStarted()
            => _service.Start(_cancellationTokenSource.Token)
                .GetAwaiter()
                .GetResult();


        private void OnStopping()
        {
            _cancellationTokenSource.Cancel();
            _service.Stop().GetAwaiter().GetResult();
        }
    }
}