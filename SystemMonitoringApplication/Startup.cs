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
            services.AddSingleton<MonitorService<MonitorSystemInfo, ProcessStateHandler>>();
            services.AddTransient<IMonitorService>(provider =>
                Environment.OSVersion.Platform switch
                {
                    PlatformID.Win32NT => provider.GetService<MonitorService<MonitorSystemInfo, ProcessStateHandler>>(),
                    _ => throw new NotImplementedException($"Реализация {nameof(IMonitorService)} для платформы {Environment.OSVersion.Platform} отсутствует!")
                }
            );
            services.AddHostedService<SystemMonitoringBackgroundWorker>();


            services.AddSingleton<IMessageProcessor, WebSocketMessageProcessor>();
            //интерфейсы доступа
            services.AddSingleton<WebSocketMessageManager>();
            services.AddControllers();
            services.AddGrpc();
            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<SystemMonitoringGrpcService>();
                endpoints.MapControllers();
            });

            app.UseWebSockets();
            app.Map("/ws", x => x.UseMiddleware<WebSocketMessageMiddleware>());
            app.UseDefaultFiles();
            app.UseStaticFiles();
        }
    }
}