using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SM.Domain;
using SM.Domain.Interfaces;

namespace SystemMonitoringApplication
{
    public class Startup
    {

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddSingleton<MonitorService<SM.Windows.MonitorSystemInfo, SM.Windows.ProcessStateHandler>>();
            services.AddTransient<IMonitorService>(provider =>
                Environment.OSVersion.Platform switch
                {
                    PlatformID.Win32NT => provider.GetService<MonitorService<SM.Windows.MonitorSystemInfo, SM.Windows.ProcessStateHandler>>(),
                    _ => throw new NotImplementedException($"Реализация {nameof(IMonitorService)} для платформы {Environment.OSVersion.Platform} отсутствует!")
                }
            );

            services.AddHostedService<SystemMonitoringBackgroundWorker>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<SystemMonitoringGrpcService>();
            });
        }
    }
}