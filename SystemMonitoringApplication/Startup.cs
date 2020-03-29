using System;
using Microsoft.Extensions.DependencyInjection;
using SM.Domain;
using SM.Domain.Interfaces;

namespace SystemMonitoringApplication
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MonitorService<SM.Windows.MonitorSystemInfo, SM.Windows.ProcessStateHandler>>();
            services.AddTransient<IMonitorService>(provider =>
                Environment.OSVersion.Platform switch
                {
                    PlatformID.Win32NT => provider.GetService<MonitorService<SM.Windows.MonitorSystemInfo, SM.Windows.ProcessStateHandler>>(),
                    _ => throw new NotImplementedException($"Реализация {nameof(IMonitorService)} для платформы {Environment.OSVersion.Platform} отсутствует!")
                }
            );

            services.AddHostedService<SystemMonitoringBackgroundService>();
        }
    }
}