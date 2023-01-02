// This software is part of the LittleBlocks.Exports Library
// Copyright (C) 2021 LittleBlocks
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
using LittleBlocks.Exports.Agent.Notifications;
using LittleBlocks.Exports.Common;
using LittleBlocks.Exports.Csv;
using LittleBlocks.Exports.Storage;
using LittleBlocks.Testing;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace LittleBlocks.Exports.Agent.UnitTests
{
    public class CsvStorageGroupExporterTests : IClassFixture<FixtureBase>
    {
        private readonly IGroupItemExporter _childExporter1;
        private readonly IGroupItemExporter _childExporter2;
        private readonly Type[] _childExporterTypes;

        private readonly FixtureBase _fixture;

        public CsvStorageGroupExporterTests(FixtureBase fixture)
        {
            _fixture = fixture;
            _childExporter1 = _fixture.Fake<IGroupItemExporter>();
            _childExporter2 = _fixture.Fake<IGroupItemExporter>();
            _childExporterTypes = new[] {_childExporter1.GetType(), _childExporter2.GetType()};
        }

        [Fact]
        public async Task Should_RunAsync_ReportsFailure_WhenExpectedChildExportersDoNotMathActualChildExporters()
        {
            // ARRANGE
            var targets = _fixture.FakeEntityList<StorageTarget>(2).ToArray();
            var reportNotifier = _fixture.Fake<IReportNotifier>();
            var reportNotifierBuilder = _fixture.Fake<IReportNotifierBuilder>();
            reportNotifierBuilder.NotificationFor(Arg.Any<string>(), Arg.Any<FailNotification>())
                .Returns(reportNotifier);
            var childExporters = new List<IGroupItemExporter> {_childExporter1};

            var sut = new SampleGroupExporter(reportNotifierBuilder, childExporters,
                _fixture.Logger<SampleGroupExporter>(), _childExporterTypes);

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
            reportNotifierBuilder.NotificationFor(Arg.Any<string>(), Arg.Any<FailNotification>())
                .Returns(reportNotifier);
            var childExporters = new List<IGroupItemExporter> {_childExporter1, _childExporter2};
            _childExporter1.RunAsync(Arg.Any<ExporterOptions>(), Arg.Any<StorageTarget[]>()).Throws(new Exception());
            var sut = new SampleGroupExporter(reportNotifierBuilder, childExporters,
                _fixture.Logger<SampleGroupExporter>(), _childExporterTypes);

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
            reportNotifierBuilder.NotificationFor(Arg.Any<string>(), Arg.Any<FailNotification>())
                .Returns(reportNotifier);
            var childExporters = new List<IGroupItemExporter> {_childExporter1, _childExporter2};

            var result1 = ExportResult.Fail("error", "childExporter1");
            _childExporter1.RunAsync(Arg.Any<ExporterOptions>(), Arg.Any<StorageTarget[]>())
                .Returns(Task.FromResult(result1));

            var result2 = ExportResult.Success("childExporter2", 125);
            _childExporter2.RunAsync(Arg.Any<ExporterOptions>(), Arg.Any<StorageTarget[]>())
                .Returns(Task.FromResult(result2));

            var sut = new SampleGroupExporter(reportNotifierBuilder, childExporters,
                _fixture.Logger<SampleGroupExporter>(), _childExporterTypes);

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
            reportNotifierBuilder.NotificationFor(Arg.Any<string>(), Arg.Any<SuccessNotification>())
                .Returns(reportNotifier);
            var childExporters = new List<IGroupItemExporter> {_childExporter1, _childExporter2};

            var result1 = ExportResult.Success("childExporter1", 45);
            _childExporter1.RunAsync(Arg.Any<ExporterOptions>(), Arg.Any<StorageTarget[]>())
                .Returns(Task.FromResult(result1));

            var result2 = ExportResult.Success("childExporter2", 125);
            _childExporter2.RunAsync(Arg.Any<ExporterOptions>(), Arg.Any<StorageTarget[]>())
                .Returns(Task.FromResult(result2));

            var sut = new SampleGroupExporter(reportNotifierBuilder, childExporters,
                _fixture.Logger<SampleGroupExporter>(), _childExporterTypes);

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
            return new(Guid.NewGuid(), Guid.NewGuid(), DateTime.Today,
                "http://localhost", "http://localhost");
        }

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

            protected override Task<(string MetadataFile, int count)> GenerateExportMetadataAsync(
                ExportResult[] results, ExporterOptions options, StorageTarget[] storageTargets)
            {
                return Task.FromResult(("Sample", 125));
            }
        }
    }
}