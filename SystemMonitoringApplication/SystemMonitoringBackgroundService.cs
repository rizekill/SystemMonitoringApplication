﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using SM.Domain.Interfaces;

namespace SystemMonitoringApplication
{
    public class SystemMonitoringBackgroundService : BackgroundService
    {
        private readonly IMonitorService _service;

        public SystemMonitoringBackgroundService(IMonitorService service) => _service = service;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
            => _service.Start(stoppingToken);

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            base.StopAsync(cancellationToken);

            _service.Stop();

            return Task.CompletedTask;
        }
    }
}