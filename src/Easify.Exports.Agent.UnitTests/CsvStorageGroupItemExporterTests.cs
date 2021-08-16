using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Easify.Testing;
using FluentAssertions;
using Easify.Exports.Csv;
using Easify.Exports.Storage;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Easify.Exports.Agent.UnitTests
{
    public class CsvStorageGroupItemExporterTests : IClassFixture<FixtureBase>
    {
        public CsvStorageGroupItemExporterTests(FixtureBase fixture)
        {
            _fixture = fixture;
        }

        private readonly FixtureBase _fixture;

        public class Sample
        {

        }

        public class SampleCsvGroupItemExporter : CsvStorageGroupItemExporter<Sample>
        {
            private readonly Func<IEnumerable<Sample>> _dataProvider;

            public SampleCsvGroupItemExporter(IFileExporter fileExporter, Func<IEnumerable<Sample>> dataProvider,
                ILogger<CsvStorageGroupItemExporter<Sample>> logger) : base(fileExporter, logger)
            {
                _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
            }

            protected override string ExportFilePrefix => "TestPrefix";

            protected override async Task<IEnumerable<Sample>> PrepareDataAsync(ExporterOptions options)
            {
                return _dataProvider();
            }
        }

        [Fact]
        public async Task Should_RunAsync_ReportSuccess_WhenThereIsDataLoaded()
        {
            // Arrange
            var fileExporter = Substitute.For<IFileExporter>();
            var logger = _fixture.Logger<SampleCsvGroupItemExporter>();
            var samples = _fixture.FakeEntityList<Sample>().ToArray();
            var storageTargets = new StorageTarget[0];

            const string testTargetFile = "testFile.csv";
            const int testRecordCount = 100;
            fileExporter.ExportAsync(Arg.Any<IEnumerable<Sample>>(), Arg.Any<ExporterOptions>()).Returns(ExportResult.Success(testTargetFile, testRecordCount));

            var sut = new SampleCsvGroupItemExporter(fileExporter, () => samples, logger);

            // Act
            var actual = await sut.RunAsync(new ExporterOptions(DateTimeOffset.Now, storageTargets, testTargetFile), storageTargets);

            // Assert
            actual.Should().NotBeNull();
            actual.HasError.Should().BeFalse();
            actual.TargetFile.Should().Be(testTargetFile);
            actual.RecordCount.Should().Be(testRecordCount);
        }

        [Fact]
         public async Task Should_RunAsync_CallExportAsync_WhenThereIsDataLoaded()
         {
             // Arrange
             var fileExporter = Substitute.For<IFileExporter>();
             var logger = _fixture.Logger<SampleCsvGroupItemExporter>();
             var samples = _fixture.FakeEntityList<Sample>().ToArray();
             var storageTargets = new StorageTarget[0];
             var sut = new SampleCsvGroupItemExporter(fileExporter, () => samples, logger);

             // Act
             await sut.RunAsync(new ExporterOptions(DateTimeOffset.Now, storageTargets, "TestPrefix"), storageTargets);

             // Assert
             await fileExporter.Received(1).ExportAsync(Arg.Any<IEnumerable<Sample>>(), Arg.Any<ExporterOptions>());
         }

         [Fact]
         public async Task Should_RunAsync_ReportsFailure_WhenThereIsErrorInLoadingData()
         {
             // Arrange
             var fileExporter = Substitute.For<IFileExporter>();
             var logger = _fixture.Logger<SampleCsvGroupItemExporter>();
             var storageTargets = new StorageTarget[0];
             var sut = new SampleCsvGroupItemExporter(fileExporter, () => null, logger);

             // Act
             var actual = await sut.RunAsync(new ExporterOptions(DateTimeOffset.Now, storageTargets, "TestPrefix"), storageTargets);

             // Assert
             actual.Should().NotBeNull();
             actual.HasError.Should().BeTrue();
             actual.RecordCount.Should().Be(0);
             actual.Error.Should().Be("Invalid data from the source.");
         }

         [Fact]
         public void Should_RunAsync_ReturnExpectedType_WhenGroupItemTypeRequested()
         {
             // Arrange
             var fileExporter = Substitute.For<IFileExporter>();
             var logger = _fixture.Logger<SampleCsvGroupItemExporter>();
             var sut = new SampleCsvGroupItemExporter(fileExporter, () => null, logger);

             // Act
             var actual = sut.GroupItemType;

             // Assert
             actual.Name.Should().Be("Sample");
         }
    }
}
