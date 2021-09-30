// This software is part of the Easify.Exports Library
// Copyright (C) 2021 Intermediate Capital Group
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

using System.Security.Principal;
using Easify.Http;
using Easify.RestEase;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Easify.Exports.Client.UnitTests
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
    }
}