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
using LittleBlocks.Exports.Csv;
using LittleBlocks.Exports.Storage;
using LittleBlocks.Testing;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace LittleBlocks.Exports.Agent.UnitTests
{
    public class CsvStorageGroupItemExporterTests : IClassFixture<FixtureBase>
    {
        private readonly FixtureBase _fixture;

        public CsvStorageGroupItemExporterTests(FixtureBase fixture)
        {
            _fixture = fixture;
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
            fileExporter.ExportAsync(Arg.Any<IEnumerable<Sample>>(), Arg.Any<ExporterOptions>())
                .Returns(ExportResult.Success(testTargetFile, testRecordCount));

            var sut = new SampleCsvGroupItemExporter(fileExporter, () => samples, logger);

            // Act
            var actual = await sut.RunAsync(new ExporterOptions(DateTimeOffset.Now, storageTargets, testTargetFile),
                storageTargets);

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
            var actual = await sut.RunAsync(new ExporterOptions(DateTimeOffset.Now, storageTargets, "TestPrefix"),
                storageTargets);

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

            protected override Task<IEnumerable<Sample>> PrepareDataAsync(ExporterOptions options)
            {
                return Task.FromResult(_dataProvider());
            }
        }
    }
}