using FluentAssertions;
using Easify.Exports.Csv;
using Xunit;

namespace Easify.Exports.UnitTests.Csv
{
    public class ExportResultTests
    {
        [Fact]
        public void Should_Fail_CreateRightExportResult()
        {
            // ARRANGE
            // ACT
            var sut = ExportResult.Fail("Error Reason");

            // ASSERT
            sut.HasError.Should().BeTrue();
            sut.Error.Should().Be("Error Reason");
            sut.RecordCount.Should().Be(0);
        }
        
        [Fact]
        public void Should_Success_CreateRightExportResult()
        {
            // ARRANGE
            // ACT
            var sut = ExportResult.Success("FilePath", 10);

            // ASSERT
            sut.HasError.Should().BeFalse();
            sut.Error.Should().BeNull();
            sut.RecordCount.Should().Be(10);
            sut.TargetFile.Should().Be("FilePath");
        }
    }
}