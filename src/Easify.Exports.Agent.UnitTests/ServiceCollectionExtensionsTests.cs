using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using Easify.Http;
using Easify.RestEase;
using FluentAssertions;
using Easify.Exports.Agent.Notifications;
using Easify.Exports.Client;
using Easify.Exports.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Easify.Exports.Agent.UnitTests
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void Should_AddExporterBuilder_AddTheBuilderCorrectlyToTheContainer()
        {
            // ARRANGE
            var services = new ServiceCollection();
            
            services.AddOptions().Configure<Exporters>(c => c.ExporterUrl1 = "http://localhost");
            services.AddTransient<IRequestContext, FakeRequestContext>();
            services.AddTransient<IRestClientBuilder, RestClientBuilder>();
            
            services.AddExporterBuilder<Exporters>((builder, c) =>
            {
                builder.AddClient("exporter#1", c.ExporterUrl1);
                builder.AddClient("exporter#2", c.ExporterUrl2);
            });

            var provider = services.BuildServiceProvider();

            var sut = provider.GetRequiredService<IExporterClientBuilder>();

            // ACT
            var exporter = sut.Build("exporter#1");

            // ASSERT
            exporter.Should().NotBeNull();
        }        
        
        [Fact]
        public void Should_AddExporters_AddTheBuilderCorrectlyToTheContainer()
        {
            // ARRANGE
            var services = new ServiceCollection();
            
            services.AddOptions().Configure<Exporters>(c => c.ExporterUrl1 = "http://localhost");
            services.AddTransient<IRequestContext, FakeRequestContext>();
            services.AddTransient<IRestClientBuilder, RestClientBuilder>();
            services.AddTransient<SampleExporter>();
            services.AddLogging();

            services.AddInMemoryStorage();
            services.AddCsv(m => { });
            services.AddExporters(m =>
            {
                m.Register<SampleExporter>(new ExportMetadata(Guid.NewGuid(), "name", "description"));
            });

            var provider = services.BuildServiceProvider();

            // ACT
            var sut = provider.GetRequiredService<SampleExporter>();

            // ASSERT
            sut.Should().NotBeNull();
        }

        public class Exporters
        {
            public string ExporterUrl1 { get; set; } = "http://localhost/export1";
            public string ExporterUrl2 { get; set; } = "http://localhost/export2";
        }

        public class FakeRequestContext : IRequestContext
        {
            public IPrincipal User { get; }
            public string CorrelationId { get; }
        }

        public class Sample
        {
            
        }

        public class SampleExporter : CsvStorageExporter<Sample>
        {
            public SampleExporter(IFileExporter fileExporter, IReportNotifierBuilder reportNotifierBuilder, ILogger<CsvStorageExporter<Sample>> logger) : base(fileExporter, reportNotifierBuilder, logger)
            {
            }

            protected override Task<IEnumerable<Sample>> PrepareDataAsync(ExportExecutionContext executionContext)
            {
                throw new System.NotImplementedException();
            }

            protected override string ExportFilePrefix => string.Empty;
        }
            
    }
}