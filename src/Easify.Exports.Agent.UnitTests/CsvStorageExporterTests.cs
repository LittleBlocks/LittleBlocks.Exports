using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Easify.Testing;
using Easify.Exports.Agent.Notifications;
using Easify.Exports.Common;
using Easify.Exports.Csv;
using Easify.Exports.Storage;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Easify.Exports.Agent.UnitTests
{
    public class CsvStorageExporterTests : IClassFixture<FixtureBase>
    {
        public CsvStorageExporterTests(FixtureBase fixture)
        {
            _fixture = fixture;
        }

        private readonly FixtureBase _fixture;

        public class Sample
        {
        }

        public class SampleCsvExporter : CsvStorageExporter<Sample>
        {
            private readonly Func<IEnumerable<Sample>> _dataProvider;

            public SampleCsvExporter(IFileExporter fileExporter, Func<IEnumerable<Sample>> dataProvider,
                IReportNotifierBuilder notifierBuilder,
                ILogger<CsvStorageExporter<Sample>> logger) : base(fileExporter, notifierBuilder, logger)
            {
                _dataProvider = dataProvider;
            }

            protected override async Task<IEnumerable<Sample>> PrepareDataAsync(ExportExecutionContext executionContext)
            {
                return _dataProvider();
            }

            protected override string ExportFilePrefix => "Sample";
        }

        [Fact]
        public async Task Should_RunAsync_ReportsFailure_WhenThereIsErrorInLoadingData()
        {
            // ARRANGE
            var context = CreateContext();
            var targets = _fixture.FakeEntityList<StorageTarget>(2).ToArray();
            var fileExporter = _fixture.Fake<IFileExporter>();
            var reportNotifier = _fixture.Fake<IReportNotifier>();
            var reportNotifierBuilder = _fixture.Fake<IReportNotifierBuilder>();
            reportNotifierBuilder.NotificationFor(Arg.Any<string>(), Arg.Any<FailNotification>())
                .Returns(reportNotifier);
            var sut = new SampleCsvExporter(fileExporter, () => null, reportNotifierBuilder,
                _fixture.Logger<SampleCsvExporter>());

            // ACT
            await sut.RunAsync(context, targets);

            // ASSERT
            await fileExporter.DidNotReceive().ExportAsync((IEnumerable<Sample>) null,
                Arg.Is<ExporterOptions>(o => o.Targets == targets));
            await reportNotifier.Received().RunAsync();
        }

        [Fact]
        public async Task Should_RunAsync_ReportsFailureAndReason_WhenTheDataIsNull()
        {
            // ARRANGE
            var exportExecutionContext = CreateContext();
            var targets = _fixture.FakeEntityList<StorageTarget>(2).ToArray();
            var context = exportExecutionContext;
            var samples = _fixture.FakeEntityList<Sample>().ToArray();
            var fileExporter = _fixture.Fake<IFileExporter>();
            fileExporter.ExportAsync(samples, Arg.Any<ExporterOptions>())
                .Returns(ExportResult.Fail("Error"));
            var reportNotifier = _fixture.Fake<IReportNotifier>();
            var reportNotifierBuilder = _fixture.Fake<IReportNotifierBuilder>();
            reportNotifierBuilder.NotificationFor(Arg.Any<string>(), Arg.Any<FailNotification>())
                .Returns(reportNotifier);
            var sut = new SampleCsvExporter(fileExporter, () => null, reportNotifierBuilder,
                _fixture.Logger<SampleCsvExporter>());

            // ACT
            await sut.RunAsync(context, targets);

            // ASSERT
            await fileExporter.DidNotReceive().ExportAsync((IEnumerable<Sample>) null,
                Arg.Is<ExporterOptions>(o => o.Targets == targets));
            await reportNotifier.Received().RunAsync();
        }

        private static ExportExecutionContext CreateContext()
        {
            return new ExportExecutionContext(Guid.NewGuid(), Guid.NewGuid(), DateTime.Today,
                "http://localhost", "http://localhost");
        }

        [Fact]
        public async Task Should_RunAsync_ReportsFailureAndReason_WhenThereIsExceptionInLoadingData()
        {
            // ARRANGE
            var context = CreateContext();
            var targets = _fixture.FakeEntityList<StorageTarget>(2).ToArray();
            var fileExporter = _fixture.Fake<IFileExporter>();
            
            var reportNotifier = _fixture.Fake<IReportNotifier>();
            var reportNotifierBuilder = _fixture.Fake<IReportNotifierBuilder>();
            reportNotifierBuilder.NotificationFor(Arg.Any<string>(), Arg.Any<FailNotification>())
                .Returns(reportNotifier);
            var sut = new SampleCsvExporter(fileExporter, () => throw new Exception("Error"), reportNotifierBuilder,
                _fixture.Logger<SampleCsvExporter>());

            // ACT
            await sut.RunAsync(context, targets);

            // ASSERT
            await fileExporter.DidNotReceive().ExportAsync((IEnumerable<Sample>) null,
                Arg.Is<ExporterOptions>(o => o.Targets == targets));
            await reportNotifier.Received().RunAsync();
        }

        [Fact]
        public async Task
            Should_RunAsync_ReportSuccessAndResultSummary_WhenTheInputsAreConfiguredCorrectly()
        {
            // ARRANGE
            var context = CreateContext();
            var targets = _fixture.FakeEntityList<StorageTarget>(2).ToArray();
            var samples = _fixture.FakeEntityList<Sample>().ToArray();
            var fileExporter = _fixture.Fake<IFileExporter>();
            fileExporter.ExportAsync(samples, Arg.Any<ExporterOptions>())
                .Returns(ExportResult.Success("targetFolder", 10));
            
            var reportNotifier = _fixture.Fake<IReportNotifier>();
            var reportNotifierBuilder = _fixture.Fake<IReportNotifierBuilder>();
            reportNotifierBuilder.NotificationFor(Arg.Any<string>(), Arg.Any<SuccessNotification>())
                .Returns(reportNotifier);

            var sut = new SampleCsvExporter(fileExporter, () => samples, reportNotifierBuilder,
                _fixture.Logger<SampleCsvExporter>());

            // ACT
            await sut.RunAsync(context, targets);

            // ASSERT
            await fileExporter.Received()
                .ExportAsync(samples, Arg.Any<ExporterOptions>());
        }
    }
}