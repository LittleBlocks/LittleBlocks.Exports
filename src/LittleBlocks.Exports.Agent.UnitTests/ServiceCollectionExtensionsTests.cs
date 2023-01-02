// This software is part of the LittleBlocks.Exports Library
// Copyright (C) 2021 LittleBlocks
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using LittleBlocks.Exports.Agent.Notifications;
using LittleBlocks.Exports.Client;
using LittleBlocks.Exports.Common;
using LittleBlocks.Http;
using LittleBlocks.RestEase;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace LittleBlocks.Exports.Agent.UnitTests
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
            public string AuthorizationHeader { get; }
        }

        public class Sample
        {
        }

        public class SampleExporter : CsvStorageExporter<Sample>
        {
            public SampleExporter(IFileExporter fileExporter, IReportNotifierBuilder reportNotifierBuilder,
                ILogger<CsvStorageExporter<Sample>> logger) : base(fileExporter, reportNotifierBuilder, logger)
            {
            }

            protected override string ExportFilePrefix => string.Empty;

            protected override Task<IEnumerable<Sample>> PrepareDataAsync(ExportExecutionContext executionContext)
            {
                throw new NotImplementedException();
            }
        }
    }
}