using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SM.Core;
using SM.Core.Web;
using SM.Domain;
using SM.Domain.Interfaces;
using SM.Windows;

namespace SystemMonitoringApplication
{
    public class Startup
    {

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IMessageProcessor, WebSocketMessageProcessor>();
            services.AddSingleton<WebSocketMessageManager>();
            services.AddGrpc();
            services.AddSingleton<MonitorService<MonitorSystemInfo, ProcessStateHandler>>();
            services.AddTransient<IMonitorService>(provider =>
                Environment.OSVersion.Platform switch
                {
                    PlatformID.Win32NT => provider.GetService<MonitorService<MonitorSystemInfo, ProcessStateHandler>>(),
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

            app.UseWebSockets();
            app.Map("/ws", x => x.UseMiddleware<WebSocketMessageMiddleware>());
            app.UseDefaultFiles();
            app.UseStaticFiles();
            
        }
    }
}