using Microsoft.Extensions.DependencyInjection;

namespace Easify.Exports.IntegrationTests.Setup
{
    public class ServiceProviderResolver : IResolver
    {
        private readonly ServiceProvider _serviceProvider;
        
        public ServiceProviderResolver() 
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddInMemoryStorage();
            services.AddCsv(c =>
            {
                c.Register<SampleEntityMap, SampleEntity>();
            });

            _serviceProvider = services.BuildServiceProvider();
        }
        
        public T Resolve<T>()
        {
            return _serviceProvider.GetRequiredService<T>();
        }
    }
}