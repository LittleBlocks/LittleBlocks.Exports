using System;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Easify.Testing;
using FluentAssertions;
using Easify.Exports.Csv;
using Easify.Exports.Extensions;
using Easify.Exports.UnitTests.Setup;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Easify.Exports.UnitTests.Csv
{
    public class CsvFileExporterTests
    {
        [Theory]
        [AutoSubstituteAndData]
        public async Task Should_ExportAsync_Succeed_WhenTheInputAndTheConfigurationIsCorrect(
            SampleEntity[] entities,
            ExporterOptions exporterOptions,
            [Frozen] ICsvFileWriter fileWriter,
            [Frozen] ICsvExportConfigurationBuilder configurationBuilder,
            CsvFileExporter sut)
        {
            // ARRANGE
            configurationBuilder.Build<SampleEntity>(exporterOptions).Returns(new CsvExportConfiguration {FileName = "FilePath"});

            // ACT
            var actual = await sut.ExportAsync(entities, exporterOptions);

            // ASSERT
            actual.Should().NotBeNull();
            actual.HasError.Should().BeFalse();
            actual.RecordCount.Should().Be(entities.Length);
            actual.TargetFile.Should().Be( "FilePath");
        }        
        
        [Theory]
        [AutoSubstituteAndData]
        public async Task Should_ExportAsync_FailWithClassMap_WhenTheInputAndTheConfigurationIsCorrect(
            SampleEntity[] entities,
            ExporterOptions exporterOptions,
            [Frozen] ICsvExportConfigurationBuilder configurationBuilder,
            CsvFileExporter sut)
        {
            // ARRANGE
            configurationBuilder.Build<SampleEntity>(exporterOptions).Returns((CsvExportConfiguration)null);

            // ACT
            var actual = await sut.ExportAsync(entities, exporterOptions);

            // ASSERT
            actual.Should().NotBeNull();
            actual.HasError.Should().BeTrue();
            actual.Error.Should().Be($"Error in creating export configuration '{typeof(SampleEntity)}'. Make sure the ClassMap has been registered");
        }        
        
        [Theory]
        [AutoSubstituteAndData]
        public async Task Should_ExportAsync_FailWithEmptyEntityList_WhenTheInputAndTheConfigurationIsCorrect(
            ExporterOptions exporterOptions,
            [Frozen] ICsvExportConfigurationBuilder configurationBuilder,
            CsvFileExporter sut)
        {
            // ARRANGE
            configurationBuilder.Build<SampleEntity>(exporterOptions).Returns(new CsvExportConfiguration {FileName = "FilePath"});

            // ACT
            var actual = await sut.ExportAsync(new SampleEntity[] {}, exporterOptions);

            // ASSERT
            actual.Should().NotBeNull();
            actual.HasError.Should().BeTrue();
            actual.Error.Should().Be($"Export for {typeof(SampleEntity)} has cancelled due to an empty list of items");
        }        
        
        [Theory]
        [AutoSubstituteAndData]
        public async Task Should_ExportAsync_FailWithError_WhenTheInputAndTheConfigurationIsCorrectButOperationThrowException(
            SampleEntity[] entities,
            ExporterOptions exporterOptions,
            [Frozen] ICsvFileWriter fileWriter,
            [Frozen] ICsvExportConfigurationBuilder configurationBuilder,
            CsvFileExporter sut)
        {
            // ARRANGE
            configurationBuilder.Build<SampleEntity>(exporterOptions).Returns(new CsvExportConfiguration {FileName = "FilePath"});
            fileWriter.WriteFileAsync(entities, Arg.Any<CsvExportConfiguration>()).Throws(new Exception("Internal Error"));

            // ACT
            var actual = await sut.ExportAsync(entities, exporterOptions);

            // ASSERT
            actual.Should().NotBeNull();
            actual.HasError.Should().BeTrue();
            actual.Error.Should().Be($"Error in exporting {typeof(SampleEntity)} to target location {exporterOptions.Targets.ToJson()}. Reason: Internal Error");
        }
    }
}