using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Easify.Exports.Csv;
using Easify.Testing;
using FluentAssertions;
using Easify.Exports.IntegrationTests.Setup;
using Easify.Exports.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Storage.Net.Blobs;
using Xunit;

namespace Easify.Exports.IntegrationTests
{
    public class CsvFileGenerationWithServiceCollectionTests : IClassFixture<FixtureBase>
    {
        private readonly FixtureBase _fixture;

        public CsvFileGenerationWithServiceCollectionTests(FixtureBase fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(Resolvers))]
        public async Task Should_GeneratedCsvFileStoredCorrectlyInStorageLocation_WhenIncomingDataIsValid(IResolver resolver)
        {
            // ARRANGE
            var entities = _fixture.FakeEntityList<SampleEntity>(5);
            var date = new DateTime(2020, 03, 31);
            var storageTargets = new[] {
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
            var fileExists = await csvStorageTarget.ExistsAsync(Path.Combine(storageTargets[0].TargetLocation, actual.TargetFile));
            fileExists.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(Resolvers))]
        public async Task Should_ThrowError_WhenIncomingDataIsEmpty(IResolver resolver)
        {
            // ARRANGE
            var entities = new SampleEntity[] { };
            var date = new DateTime(2020, 03, 31);
            var storageTargets = new[] {
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
            var storageTargets = new[] {
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
            actual.Error.Should().Be($"Error in creating export configuration '{typeof(SampleEntity2)}'. Make sure the ClassMap has been registered");
        }

        // [Fact]
        // TODO: Should be revised
        public async Task Should_ExportAsync_UploadTheFileInAws()
        {
            var configPath = Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), "appsettings.json");
            var entities = _fixture.FakeEntityList<SampleEntity>(5);
            var configuration = new ConfigurationBuilder().AddJsonFile(configPath, false).Build();
            var storageTargets = new[] {
                new StorageTarget
                {
                    TargetLocation = "Mezz/Valuations/Automated/", StorageTargetType = StorageTargetType.S3Bucket
                }
            };
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddS3BucketStorageWithSamlSupport(configuration);
            services.AddCsv(c =>
            {
                c.Register<SampleEntityMap, SampleEntity>();
            });

            var serviceProvider = services.BuildServiceProvider();
            var sut = serviceProvider.GetRequiredService<IFileExporter>();

            var actual = await sut.ExportAsync(entities, new ExporterOptions(DateTime.Today, storageTargets));

            actual.Should().NotBe(null);
        }
        
        [Fact]
        public async Task Should_ExportAsync_UploadTheFileInAzureBlobStorageWithSharedKey()
        {
            var configPath = Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), "appsettings.json");
            var entities = _fixture.FakeEntityList<SampleEntity>(5);
            var configuration = new ConfigurationBuilder().AddJsonFile(configPath, false).Build();
            var storageTargets = new[] {
                new StorageTarget
                {
                    TargetLocation = "mezz/valuations/automated/", StorageTargetType = StorageTargetType.BlobStorage
                }
            };
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddAccountWithSharedKeyStorage(configuration);
            services.AddCsv(c =>
            {
                c.Register<SampleEntityMap, SampleEntity>();
            });
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
            var configPath = Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), "appsettings.json");
            var entities = _fixture.FakeEntityList<SampleEntity>(5);
            var configuration = new ConfigurationBuilder().AddJsonFile(configPath, false).Build();
            var storageTargets = new[] {
                new StorageTarget
                {
                    TargetLocation = "mezz/valuations/manual/", StorageTargetType = StorageTargetType.BlobStorage
                }
            };
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddAccountWithAzureAdStorage(configuration);
            services.AddCsv(c =>
            {
                c.Register<SampleEntityMap, SampleEntity>();
            });
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
            var storageTargets = new[] {
                new StorageTarget
                {
                    TargetLocation = "mezz/valuations/manual/", StorageTargetType = StorageTargetType.BlobStorage
                }
            };
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddAccountWithEmulatorStorage();
            services.AddCsv(c =>
            {
                c.Register<SampleEntityMap, SampleEntity>();
            });

            var serviceProvider = services.BuildServiceProvider();
            var sut = serviceProvider.GetRequiredService<IFileExporter>();

            var actual = await sut.ExportAsync(entities, new ExporterOptions(DateTime.Today, storageTargets));

            actual.Should().NotBe(null);
            actual.HasError.Should().BeFalse();
        }

        public static IEnumerable<object[]> Resolvers => new List<object[]>
        {
            new object[] {new AutofacResolver()},
            new object[] {new ServiceProviderResolver()}
        };
    }
}