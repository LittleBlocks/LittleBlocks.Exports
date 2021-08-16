using System;
using Easify.Exports.Agent.Notifications;
using Microsoft.Extensions.DependencyInjection;

namespace Easify.Exports.Agent
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddExporters(this IServiceCollection services, Action<IExporterRegistry> configure)
        {
            services.AddSingleton<IReportNotifierBuilder, WebHookNotifierBuilder>();
            services.AddSingleton(sp =>
            {
                var registry = new ExporterRegistry(sp);
                
                configure?.Invoke(registry);

                return registry;
            });
            services.AddSingleton<IExporterBuilder>(sp => sp.GetRequiredService<ExporterRegistry>());
            services.AddSingleton<IExporterRegistry>(sp => sp.GetRequiredService<ExporterRegistry>());
            return services;
        }
    }
}