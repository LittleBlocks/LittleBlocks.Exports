using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Easify.Testing;
using Easify.Exports.Storage;
using NSubstitute;
using Storage.Net.Blobs;
using Xunit;

namespace Easify.Exports.UnitTests.Storage
{
    public class CsvStorageTargetTests
    {
        [Theory]
        [AutoSubstituteAndData]
        public async Task Should_Resolve_ReturnCorrectStorageTarget_WhenItIsPresented(IBlobStorage blobStorage)
        {
            // Arrange
            var sut = new GenericCsvStorageTarget(StorageTargetType.InMemory, blobStorage);
            var expected = "Mezz/Valuation/File.csv";

            // Act
            await sut.WriteAsync("Mezz/Valuation/", "File.csv", new byte[] {1, 12, 123});

            // Assert
            await blobStorage.Received(1)
                .WriteAsync(expected, Arg.Any<Stream>(), Arg.Any<bool>(), Arg.Any<CancellationToken>());
        }        

    }
}