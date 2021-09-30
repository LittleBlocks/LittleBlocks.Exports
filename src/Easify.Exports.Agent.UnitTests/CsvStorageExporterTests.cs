// This software is part of the Easify.Exports Library
// Copyright (C) 2021 Intermediate Capital Group
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Easify.Exports.Agent.Notifications;
using Easify.Exports.Common;
using Easify.Exports.Csv;
using Easify.Exports.Storage;
using Easify.Testing;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Easify.Exports.Agent.UnitTests
{
    public class CsvStorageExporterTests : IClassFixture<FixtureBase>
    {
        private readonly FixtureBase _fixture;

        public CsvStorageExporterTests(FixtureBase fixture)
        {
            _fixture = fixture;
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
        
        
        private static ExportExecutionContext CreateContext()
        {
            return new(Guid.NewGuid(), Guid.NewGuid(), DateTime.Today,
                "http://localhost", "http://localhost");
        }

        public class Sample
        {
        }

        public class SampleCsvExporter : CsvStorageExporter<Sample>
        {
            private readonly Func<IEnumerable<Sample>> _dataProvider;

            public SampleCsvExporter(IFileExporter fileExporter, Func<IEnumerable<Sample>> dataProvider,
                IReportNotifierBuilder notifierBuilder,
                ILogger<SampleCsvExporter> logger) : base(fileExporter, notifierBuilder, logger)
            {
                _dataProvider = dataProvider;
            }

            protected override string ExportFilePrefix => "Sample";

            protected override Task<IEnumerable<Sample>> PrepareDataAsync(ExportExecutionContext executionContext)
            {
                return Task.FromResult(_dataProvider());
            }
        }
    }
}