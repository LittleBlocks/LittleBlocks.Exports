using System;
using Easify.Testing;
using FluentAssertions;
using Easify.Exports.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Easify.Exports.UnitTests
{
    public class ServiceCollectionExtensionsTests : IClassFixture<FixtureBase>
    {
        private readonly FixtureBase _fixture;

        public ServiceCollectionExtensionsTests(FixtureBase fixture)
        {
            _fixture = fixture;
        }
        
        [Fact]
        public void Should_AddLocalDiskStorage_RegisterBlobStorageCorrectly()
        {
            // ARRANGE
            var configuration = _fixture.Fake<IConfiguration>();
            configuration.GetSection(Arg.Any<string>()).Returns(_fixture.Fake<IConfigurationSection>());

            var services = new ServiceCollection();
            services.AddLocalDiskStorage();

            var provider = services.BuildServiceProvider();

            // ACT
            var sut = provider.GetRequiredService<ICsvStorageTargetResolver>();

            // ASSERT
            sut.Resolve(StorageTargetType.LocalDisk).Should().NotBeNull();

            sut.Resolve(StorageTargetType.InMemory).Should().BeNull();
            sut.Resolve(StorageTargetType.S3Bucket).Should().BeNull();
        } 
        
        [Fact]
        public void Should_AddInMemoryStorage_RegisterBlobStorageCorrectly()
        {
            // ARRANGE
            var configuration = _fixture.Fake<IConfiguration>();
            configuration.GetSection(Arg.Any<string>()).Returns(_fixture.Fake<IConfigurationSection>());

            var services = new ServiceCollection();
            services.AddInMemoryStorage();

            var provider = services.BuildServiceProvider();

            // ACT
            var sut = provider.GetRequiredService<ICsvStorageTargetResolver>();

            // ASSERT
            sut.Resolve(StorageTargetType.InMemory).Should().NotBeNull();

            sut.Resolve(StorageTargetType.LocalDisk).Should().BeNull();            
            sut.Resolve(StorageTargetType.S3Bucket).Should().BeNull();
        } 
    }
}