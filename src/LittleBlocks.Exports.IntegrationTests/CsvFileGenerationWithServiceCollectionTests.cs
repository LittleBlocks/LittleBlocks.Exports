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
using System.IO;
using System.Threading.Tasks;
using LittleBlocks.Exports.Csv;
using LittleBlocks.Exports.IntegrationTests.Setup;
using LittleBlocks.Exports.Storage;
using LittleBlocks.Testing;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LittleBlocks.Exports.IntegrationTests
{
    public class CsvFileGenerationWithServiceCollectionTests : IClassFixture<FixtureBase>
    {
        private readonly FixtureBase _fixture;

        public CsvFileGenerationWithServiceCollectionTests(FixtureBase fixture)
        {
            _fixture = fixture;
        }

        public static IEnumerable<object[]> Resolvers => new List<object[]>
        {
            new object[] {new AutofacResolver()},
            new object[] {new ServiceProviderResolver()}
        };

        [Theory]
        [MemberData(nameof(Resolvers))]
        public async Task Should_GeneratedCsvFileStoredCorrectlyInStorageLocation_WhenIncomingDataIsValid(
            IResolver resolver)
        {
            // ARRANGE
            var entities = _fixture.FakeEntityList<SampleEntity>(5);
            var date = new DateTime(2020, 03, 31);
            var storageTargets = new[]
            {
                new StorageTarget
                {
                    TargetLocation = "SampleLocation\\Sample Folder\\", StorageTargetType = StorageTargetType.InMemory
                }
            };

            var csvStorageTarget = resolver.Resolve<ICsvStorageTarget>();
            var sut = resolver.Resolve<IFileExporter>();

            // ACT
            var actual = await sut.ExportAsync(entities, new ExporterOptions(date, storageTargets, "sample"));

            // ASSERT
            actual.HasError.Should().BeFalse();
            actual.RecordCount.Should().Be(5);
            actual.TargetFile.Should().Be($"sample{date:yyyyMMddHHmmss}.csv");
            var fileExists =
                await csvStorageTarget.ExistsAsync(Path.Combine(storageTargets[0].TargetLocation, actual.TargetFile));
            fileExists.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(Resolvers))]
        public async Task Should_ThrowError_WhenIncomingDataIsEmpty(IResolver resolver)
        {
            // ARRANGE
            var entities = new SampleEntity[] { };
            var date = new DateTime(2020, 03, 31);
            var storageTargets = new[]
            {
                new StorageTarget
                {
                    TargetLocation = "SampleLocation\\Sample Folder\\", StorageTargetType = StorageTargetType.InMemory
                }
            };

            var sut = resolver.Resolve<IFileExporter>();

            // ACT
            var actual = await sut.ExportAsync(entities, new ExporterOptions(date, storageTargets));

            // ASSERT
            actual.HasError.Should().BeTrue();
            actual.RecordCount.Should().Be(0);
            actual.Error.Should().Be($"Export for {typeof(SampleEntity)} has cancelled due to an empty list of items");
        }

        [Theory]
        [MemberData(nameof(Resolvers))]
        public async Task Should_ThrowError_WhenTheConfigurationDoesntHaveMappingForTheType(IResolver resolver)
        {
            // ARRANGE
            var entities = _fixture.FakeEntityList<SampleEntity2>(5);
            var date = new DateTime(2020, 03, 31);
            var storageTargets = new[]
            {
                new StorageTarget
                {
                    TargetLocation = "SampleLocation\\Sample Folder\\", StorageTargetType = StorageTargetType.InMemory
                }
            };

            var sut = resolver.Resolve<IFileExporter>();

            // ACT
            var actual = await sut.ExportAsync(entities, new ExporterOptions(date, storageTargets));

            // ASSERT
            actual.HasError.Should().BeTrue();
            actual.RecordCount.Should().Be(0);
            actual.Error.Should()
                .Be(
                    $"Error in creating export configuration '{typeof(SampleEntity2)}'. Make sure the ClassMap has been registered");
        }

        // [Fact]
        // TODO: Should be revised
        public async Task Should_ExportAsync_UploadTheFileInAws()
        {
            var configPath = Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location) ?? string.Empty,
                "appsettings.json");
            var entities = _fixture.FakeEntityList<SampleEntity>(5);
            var configuration = new ConfigurationBuilder().AddJsonFile(configPath, false).Build();
            var storageTargets = new[]
            {
                new StorageTarget
                {
                    TargetLocation = "Mezz/Valuations/Automated/", StorageTargetType = StorageTargetType.S3Bucket
                }
            };
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddS3BucketStorageWithSamlSupport(configuration);
            services.AddCsv(c => { c.Register<SampleEntityMap, SampleEntity>(); });

            var serviceProvider = services.BuildServiceProvider();
            var sut = serviceProvider.GetRequiredService<IFileExporter>();

            var actual = await sut.ExportAsync(entities, new ExporterOptions(DateTime.Today, storageTargets));

            actual.Should().NotBe(null);
        }

        [Fact]
        public async Task Should_ExportAsync_UploadTheFileInAzureBlobStorageWithSharedKey()
        {
            var configPath = Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location) ?? string.Empty,
                "appsettings.json");
            var entities = _fixture.FakeEntityList<SampleEntity>(5);
            var configuration = new ConfigurationBuilder().AddJsonFile(configPath, false).Build();
            var storageTargets = new[]
            {
                new StorageTarget
                {
                    TargetLocation = "mezz/valuations/automated/", StorageTargetType = StorageTargetType.BlobStorage
                }
            };
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddAccountWithSharedKeyStorage(configuration);
            services.AddCsv(c => { c.Register<SampleEntityMap, SampleEntity>(); });
            services.AddTransient(sp => _fixture.Fake<ICsvFileWriter>());

            var serviceProvider = services.BuildServiceProvider();
            var sut = serviceProvider.GetRequiredService<IFileExporter>();

            var actual = await sut.ExportAsync(entities, new ExporterOptions(DateTime.Today, storageTargets));

            actual.Should().NotBe(null);
            actual.HasError.Should().BeFalse();
        }

        [Fact]
        public async Task Should_ExportAsync_UploadTheFileInAzureBlobStorageWithAzureAd()
        {
            var configPath = Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location) ?? string.Empty,
                "appsettings.json");
            var entities = _fixture.FakeEntityList<SampleEntity>(5);
            var configuration = new ConfigurationBuilder().AddJsonFile(configPath, false).Build();
            var storageTargets = new[]
            {
                new StorageTarget
                {
                    TargetLocation = "mezz/valuations/manual/", StorageTargetType = StorageTargetType.BlobStorage
                }
            };
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddAccountWithAzureAdStorage(configuration);
            services.AddCsv(c => { c.Register<SampleEntityMap, SampleEntity>(); });
            services.AddTransient(sp => _fixture.Fake<ICsvFileWriter>());

            var serviceProvider = services.BuildServiceProvider();
            var sut = serviceProvider.GetRequiredService<IFileExporter>();

            var actual = await sut.ExportAsync(entities, new ExporterOptions(DateTime.Today, storageTargets));

            actual.Should().NotBe(null);
            actual.HasError.Should().BeFalse();
        }

        //[Fact]
        public async Task Should_ExportAsync_UploadTheFileInAzureBlobStorageWithEmulator()
        {
            var entities = _fixture.FakeEntityList<SampleEntity>(5);
            var storageTargets = new[]
            {
                new StorageTarget
                {
                    TargetLocation = "mezz/valuations/manual/", StorageTargetType = StorageTargetType.BlobStorage
                }
            };
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddAccountWithEmulatorStorage();
            services.AddCsv(c => { c.Register<SampleEntityMap, SampleEntity>(); });

            var serviceProvider = services.BuildServiceProvider();
            var sut = serviceProvider.GetRequiredService<IFileExporter>();

            var actual = await sut.ExportAsync(entities, new ExporterOptions(DateTime.Today, storageTargets));

            actual.Should().NotBe(null);
            actual.HasError.Should().BeFalse();
        }
    }
}