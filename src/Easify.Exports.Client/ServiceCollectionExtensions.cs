using System;
using Easify.RestEase;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Easify.Exports.Client
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddExporterBuilder<TClients>(this IServiceCollection services,
            Action<IExporterClientRegistry, TClients> configure)
            where TClients : class, new()
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            services.AddSingleton<IExporterClientBuilder>(sp =>
            {
                var clients = sp.GetRequiredService<IOptions<TClients>>().Value;

                var builder = new ExporterClientBuilder();
                configure?.Invoke(builder, clients);

                return builder;
            });

            return services;
        }
    }
}