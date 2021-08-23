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
using Easify.Exports.Csv;
using Easify.Exports.Storage;
using Easify.Testing;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Easify.Exports.Agent.UnitTests
{
    public class CsvStorageGroupViewExporterTests : IClassFixture<FixtureBase>
    {
        private readonly FixtureBase _fixture;

        public CsvStorageGroupViewExporterTests(FixtureBase fixture)
        {
            _fixture = fixture;
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
            fileExporter.ExportAsync(Arg.Any<IEnumerable<Sample>>(), Arg.Any<ExporterOptions>())
                .Returns(ExportResult.Success(testTargetFile, testRecordCount));

            var sut = new SampleCsvStorageGroupViewExporter(fileExporter, () => samples, logger);

            // Act
            var actual = await sut.RunAsync(new ExporterOptions(DateTimeOffset.Now, storageTargets, testTargetFile),
                storageTargets);

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
            fileExporter.ExportAsync(Arg.Any<IEnumerable<Sample>>(), Arg.Any<ExporterOptions>())
                .Returns(ExportResult.Success(testTargetFile, testRecordCount));

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
            var actual = await sut.RunAsync(new ExporterOptions(DateTimeOffset.Now, storageTargets, "TestPrefix"),
                storageTargets);

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
            var actual = await sut.RunAsync(new ExporterOptions(DateTimeOffset.Now, storageTargets, "TestPrefix"),
                storageTargets);

            // Assert
            actual.Should().NotBeNull();
            actual.HasError.Should().BeTrue();
            actual.RecordCount.Should().Be(0);
            actual.Error.Should().Be("Invalid data from the source.");
        }

        public class Sample
        {
        }

        public class SampleCsvStorageGroupViewExporter : CsvStorageGroupViewExporter<Sample>
        {
            private readonly Func<ViewExportResult<Sample>> _dataProvider;

            public SampleCsvStorageGroupViewExporter(IFileExporter fileExporter,
                Func<ViewExportResult<Sample>> dataProvider,
                ILogger<CsvStorageGroupViewExporter<Sample>> logger) : base(fileExporter, logger)
            {
                _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
            }

            protected override string[] Schemas => new[] {"schema1", "schema2"};
            protected override string ViewPrefix => "vw_";

            protected override Task<ViewExportResult<Sample>> PrepareDataAsync(string viewPrefix, string schema,
                string viewName, ExporterOptions options)
            {
                return Task.FromResult(_dataProvider());
            }
        }
    }
}