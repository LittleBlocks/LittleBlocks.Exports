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
    public class CsvStorageGroupViewExporterTests : IClassFixture<FixtureBase>
    {
        public CsvStorageGroupViewExporterTests(FixtureBase fixture)
        {
            _fixture = fixture;
        }

        private readonly FixtureBase _fixture;

        public class Sample
        {

        }

        public class SampleCsvStorageGroupViewExporter : CsvStorageGroupViewExporter<Sample>
        {
            private readonly Func<ViewExportResult<Sample>> _dataProvider;

            public SampleCsvStorageGroupViewExporter(IFileExporter fileExporter, Func<ViewExportResult<Sample>> dataProvider,
                ILogger<CsvStorageGroupViewExporter<Sample>> logger) : base(fileExporter, logger)
            {
                _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
            }

            protected override string[] Schemas => new[] {"schema1", "schema2"};
            protected override string ViewPrefix => "vw_";
            protected override async Task<ViewExportResult<Sample>> PrepareDataAsync(string viewPrefix, string schema, string viewName, ExporterOptions options)
            {
                return _dataProvider();
            }
        }

        [Fact]
        public async Task Should_RunAsync_ReportSuccess_WhenThereIsDataLoaded()
        {
            // Arrange
            var fileExporter = Substitute.For<IFileExporter>();
            var logger = _fixture.Logger<SampleCsvStorageGroupViewExporter>();
            var samples = _fixture.FakeEntity<ViewExportResult<Sample>>();
            var storageTargets = new StorageTarget[0];

            const string testTargetFile = "testFile.csv";
            const int testRecordCount = 100;
            fileExporter.ExportAsync(Arg.Any<IEnumerable<Sample>>(), Arg.Any<ExporterOptions>()).Returns(ExportResult.Success(testTargetFile, testRecordCount));

            var sut = new SampleCsvStorageGroupViewExporter(fileExporter, () => samples, logger);

            // Act
            var actual = await sut.RunAsync(new ExporterOptions(DateTimeOffset.Now, storageTargets, testTargetFile), storageTargets);

            // Assert
            actual.Should().NotBeNull();
            actual.HasError.Should().BeFalse();
            actual.RecordCount.Should().Be(2);
        }

        [Fact]
        public async Task Should_RunAsync_CallExportAsync_WhenThereIsDataLoaded()
        {
            // Arrange
            var fileExporter = Substitute.For<IFileExporter>();
            var logger = _fixture.Logger<SampleCsvStorageGroupViewExporter>();
            var samples = _fixture.FakeEntity<ViewExportResult<Sample>>();
            var storageTargets = new StorageTarget[0];

            const string testTargetFile = "testFile.csv";
            const int testRecordCount = 100;
            fileExporter.ExportAsync(Arg.Any<IEnumerable<Sample>>(), Arg.Any<ExporterOptions>()).Returns(ExportResult.Success(testTargetFile, testRecordCount));

            var sut = new SampleCsvStorageGroupViewExporter(fileExporter, () => samples, logger);

            // Act
            await sut.RunAsync(new ExporterOptions(DateTimeOffset.Now, storageTargets, "TestPrefix"), storageTargets);

            // Assert
            await fileExporter.Received(2).ExportAsync(Arg.Any<IEnumerable<Sample>>(), Arg.Any<ExporterOptions>());
        }

        [Fact]
        public async Task Should_RunAsync_ReportsFailure_WhenThereIsErrorInLoadingData()
        {
            // Arrange
            var fileExporter = Substitute.For<IFileExporter>();
            var logger = _fixture.Logger<SampleCsvStorageGroupViewExporter>();
            var storageTargets = new StorageTarget[0];
            var sut = new SampleCsvStorageGroupViewExporter(fileExporter, () => null, logger);

            // Act
            var actual = await sut.RunAsync(new ExporterOptions(DateTimeOffset.Now, storageTargets, "TestPrefix"), storageTargets);

            // Assert
            actual.Should().NotBeNull();
            actual.HasError.Should().BeTrue();
            actual.RecordCount.Should().Be(0);
            actual.Error.Should().Be("Invalid data from the source.");
        }

        [Fact]
        public async Task Should_RunAsync_ReportsFailure_WhenNoDataIsExportedForAnySchema()
        {
            // Arrange
            var fileExporter = Substitute.For<IFileExporter>();
            var logger = _fixture.Logger<SampleCsvStorageGroupViewExporter>();
            var storageTargets = new StorageTarget[0];
            var sut = new SampleCsvStorageGroupViewExporter(fileExporter, () => new ViewExportResult<Sample>(), logger);

            // Act
            var actual = await sut.RunAsync(new ExporterOptions(DateTimeOffset.Now, storageTargets, "TestPrefix"), storageTargets);

            // Assert
            actual.Should().NotBeNull();
            actual.HasError.Should().BeTrue();
            actual.RecordCount.Should().Be(0);
            actual.Error.Should().Be("Invalid data from the source.");
        }
    }
}
