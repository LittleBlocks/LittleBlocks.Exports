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
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Easify.Exports.Agent.UnitTests
{
    public class CsvStorageGroupExporterTests : IClassFixture<FixtureBase>
    {
        readonly Type[] _childExporterTypes;
        readonly IGroupItemExporter _childExporter1;
        readonly IGroupItemExporter _childExporter2;

        public CsvStorageGroupExporterTests(FixtureBase fixture)
        {
            _fixture = fixture;
            _childExporter1 = _fixture.Fake<IGroupItemExporter>();
            _childExporter2 = _fixture.Fake<IGroupItemExporter>();
            _childExporterTypes = new Type[] {_childExporter1.GetType() , _childExporter2.GetType() };
        }

        private readonly FixtureBase _fixture;

        public class SampleGroupExporter : CsvStorageGroupExporter
        {
            public SampleGroupExporter(IReportNotifierBuilder reportNotifierBuilder,
                IEnumerable<IGroupItemExporter> groupItemExporters, ILogger<CsvStorageGroupExporter> logger,
                Type[] childExporterTypes) : base(
                reportNotifierBuilder, groupItemExporters, logger)
            {
                ChildExporterTypes = childExporterTypes;
            }

            protected override Type[] ChildExporterTypes { get; }

            protected override string ExportFilePrefix => "Sample";
            protected override Task<(string MetadataFile, int count)> GenerateExportMetadataAsync(ExportResult[] results, ExporterOptions options, StorageTarget[] storageTargets)
            {
               return Task.FromResult(("Sample", 125));
            }
        }

        [Fact]
        public async Task Should_RunAsync_ReportsFailure_WhenExpectedChildExportersDoNotMathActualChildExporters()
        {
            // ARRANGE
            var targets = _fixture.FakeEntityList<StorageTarget>(2).ToArray();
            var reportNotifier = _fixture.Fake<IReportNotifier>();
            var reportNotifierBuilder = _fixture.Fake<IReportNotifierBuilder>();
            reportNotifierBuilder.NotificationFor(Arg.Any<string>(), Arg.Any<FailNotification>()).Returns(reportNotifier);
            var childExporters = new List<IGroupItemExporter>{ _childExporter1 };

            var sut = new SampleGroupExporter(reportNotifierBuilder, childExporters, _fixture.Logger<SampleGroupExporter>(), _childExporterTypes);

            // ACT
            await sut.RunAsync(CreateContext(), targets);

            // ASSERT
            await _childExporter1.DidNotReceive().RunAsync(Arg.Is<ExporterOptions>(o => o.Targets == targets), 
                Arg.Is<StorageTarget[]>(t => t == targets));
            await reportNotifier.Received().RunAsync();
        }

        [Fact]
        public async Task Should_RunAsync_ReportsFailure_WhenChildExporterThrowsException()
        {
            // ARRANGE
            var targets = _fixture.FakeEntityList<StorageTarget>(2).ToArray();
            var reportNotifier = _fixture.Fake<IReportNotifier>();
            var reportNotifierBuilder = _fixture.Fake<IReportNotifierBuilder>();
            reportNotifierBuilder.NotificationFor(Arg.Any<string>(), Arg.Any<FailNotification>()).Returns(reportNotifier);
            var childExporters = new List<IGroupItemExporter> { _childExporter1, _childExporter2 };
            _childExporter1.RunAsync(Arg.Any<ExporterOptions>(), Arg.Any<StorageTarget[]>()).Throws(new Exception());
            var sut = new SampleGroupExporter(reportNotifierBuilder, childExporters, _fixture.Logger<SampleGroupExporter>(), _childExporterTypes);

            // ACT
            await sut.RunAsync(CreateContext(), targets);

            // ASSERT
            await _childExporter1.Received().RunAsync(Arg.Is<ExporterOptions>(o => o.Targets == targets),
                Arg.Is<StorageTarget[]>(t => t == targets));
            await reportNotifier.Received().RunAsync();
        }

        [Fact]
        public async Task Should_RunAsync_ReportsFailure_WhenChildExporterIsNotSuccessful()
        {
            // ARRANGE
            var targets = _fixture.FakeEntityList<StorageTarget>(2).ToArray();
            var reportNotifier = _fixture.Fake<IReportNotifier>();
            var reportNotifierBuilder = _fixture.Fake<IReportNotifierBuilder>();
            reportNotifierBuilder.NotificationFor(Arg.Any<string>(), Arg.Any<FailNotification>()).Returns(reportNotifier);
            var childExporters = new List<IGroupItemExporter> { _childExporter1, _childExporter2 };

            var result1 = ExportResult.Fail("error", "childExporter1");
            _childExporter1.RunAsync(Arg.Any<ExporterOptions>(), Arg.Any<StorageTarget[]>()).Returns(Task.FromResult(result1));

            var result2 = ExportResult.Success("childExporter2", 125);
            _childExporter2.RunAsync(Arg.Any<ExporterOptions>(), Arg.Any<StorageTarget[]>()).Returns(Task.FromResult(result2));

            var sut = new SampleGroupExporter(reportNotifierBuilder, childExporters, _fixture.Logger<SampleGroupExporter>(), _childExporterTypes);

            // ACT
            await sut.RunAsync(CreateContext(), targets);

            // ASSERT
            await _childExporter1.Received().RunAsync(Arg.Is<ExporterOptions>(o => o.Targets == targets),
                Arg.Is<StorageTarget[]>(t => t == targets));
            await _childExporter2.Received().RunAsync(Arg.Is<ExporterOptions>(o => o.Targets == targets),
                Arg.Is<StorageTarget[]>(t => t == targets));
            await reportNotifier.Received().RunAsync();
        }

        [Fact]
        public async Task Should_RunAsync_ReportsSuccess_WhenChildExportersAreNotSuccessful()
        {
            // ARRANGE
            var context = CreateContext();
            var targets = _fixture.FakeEntityList<StorageTarget>(2).ToArray();
            var reportNotifier = _fixture.Fake<IReportNotifier>();
            var reportNotifierBuilder = _fixture.Fake<IReportNotifierBuilder>();
            reportNotifierBuilder.NotificationFor(Arg.Any<string>(), Arg.Any<SuccessNotification>()).Returns(reportNotifier);
            var childExporters = new List<IGroupItemExporter> { _childExporter1, _childExporter2 };

            var result1 = ExportResult.Success("childExporter1", 45);
            _childExporter1.RunAsync(Arg.Any<ExporterOptions>(), Arg.Any<StorageTarget[]>()).Returns(Task.FromResult(result1));

            var result2 = ExportResult.Success("childExporter2", 125);
            _childExporter2.RunAsync(Arg.Any<ExporterOptions>(), Arg.Any<StorageTarget[]>()).Returns(Task.FromResult(result2));

            var sut = new SampleGroupExporter(reportNotifierBuilder, childExporters, _fixture.Logger<SampleGroupExporter>(), _childExporterTypes);

            // ACT
            await sut.RunAsync(context, targets);

            // ASSERT
            await _childExporter1.Received().RunAsync(Arg.Is<ExporterOptions>(o => o.Targets == targets),
                Arg.Is<StorageTarget[]>(t => t == targets));
            await _childExporter2.Received().RunAsync(Arg.Is<ExporterOptions>(o => o.Targets == targets),
                Arg.Is<StorageTarget[]>(t => t == targets));
            await reportNotifier.Received().RunAsync();
        }

        private static ExportExecutionContext CreateContext()
        {
            return new ExportExecutionContext(Guid.NewGuid(), Guid.NewGuid(), DateTime.Today,
                "http://localhost", "http://localhost");
        }
    }
}