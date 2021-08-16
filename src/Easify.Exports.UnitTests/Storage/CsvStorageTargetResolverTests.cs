using Easify.Testing;
using FluentAssertions;
using Easify.Exports.Storage;
using Storage.Net.Blobs;
using Xunit;

namespace Easify.Exports.UnitTests.Storage
{
    public class CsvStorageTargetResolverTests
    {
        [Theory]
        [AutoSubstituteAndData]
        public void Should_Resolve_ReturnCorrectStorageTarget_WhenItIsPresented(IBlobStorage blobStorage)
        {
            // Arrange
            var sut = new CsvStorageTargetResolver(new []{new GenericCsvStorageTarget(StorageTargetType.InMemory, blobStorage)});

            // Act
            var actual = sut.Resolve(StorageTargetType.InMemory);

            // Assert
            actual.Should().NotBeNull();
        }        
        
        [Theory]
        [AutoSubstituteAndData]
        public void Should_Resolve_ReturnNull_WhenItIsNotPresented(IBlobStorage blobStorage)
        {
            // Arrange
            var sut = new CsvStorageTargetResolver(new []{new GenericCsvStorageTarget(StorageTargetType.InMemory, blobStorage)});

            // Act
            var actual = sut.Resolve(StorageTargetType.LocalDisk);

            // Assert
            actual.Should().BeNull();
        }
    }
}