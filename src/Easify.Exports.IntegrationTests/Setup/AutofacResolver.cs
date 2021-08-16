using Autofac;
using Autofac.Extensions.DependencyInjection;
using Easify.Exports.Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace Easify.Exports.IntegrationTests.Setup
{
    public class AutofacResolver : IResolver
    {
        private IContainer _container;

        public AutofacResolver()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            
            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.RegisterInMemoryStorage();
            builder.RegisterCsv(c =>
            {
                c.Register<SampleEntityMap, SampleEntity>();
            });

            _container = builder.Build();
        }
        public T Resolve<T>()
        {
            return _container.Resolve<T>();
        }
    }
}