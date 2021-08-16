using System;
using AutoFixture.Xunit2;
using CsvHelper.Configuration;
using Easify.Testing;
using FluentAssertions;
using Easify.Exports.Csv;
using Easify.Exports.Storage;
using Easify.Exports.UnitTests.Setup;
using NSubstitute;
using Xunit;

namespace Easify.Exports.UnitTests.Csv
{
    public class CsvExportConfigurationBuilderTests
    {
        [Theory]
        [AutoSubstituteAndData]
        public void Should_CsvFileWriterContextBuilder_ReturnsCorrectCsvFileWriterContext_WhenItContainsValidExportDataAndClassMap(
            [Frozen] ICsvContextMapResolver contextMapResolver,
            [Frozen] IExportFileNameBuilder exportFileNameBuilder,
            StorageTarget[] targets,
            CsvExportConfigurationBuilder sut
            )
        {
            // Arrange
            var exporterOptions = new ExporterOptions(DateTime.Today, targets);
            contextMapResolver.Resolve<SampleEntity>().Returns(typeof(SampleEntityMap));
            exportFileNameBuilder.Build(exporterOptions).Returns("20200101.csv");
                
            // Act
            var actual = sut.Build<SampleEntity>(exporterOptions);
            
            // Assert
            actual.Should().NotBeNull();
            actual.Targets.Should().BeEquivalentTo(exporterOptions.Targets);
            actual.FileName.Should().Be($"20200101.csv");
            actual.Configuration.Delimiter.Should().Be("|");
            actual.Configuration.HasHeaderRecord.Should().BeTrue();
            actual.Configuration.TrimOptions.Should().Be(TrimOptions.InsideQuotes);
        }        
        
        [Theory]
        [AutoSubstituteAndData]
        public void Should_CsvFileWriterContextBuilder_ReturnsNull_WhenItContainsValidExportDataButNotClassMap(
            [Frozen] IExportFileNameBuilder exportFileNameBuilder,
            ExporterOptions exporterOptions,
            CsvExportConfigurationBuilder sut
            )
        {
            // Arrange
            exportFileNameBuilder.Build(exporterOptions).Returns("20200101.csv");
                
            // Act
            var actual = sut.Build<SampleEntity>(exporterOptions);
            
            // Assert
            actual.Should().BeNull();
        }
    }
}