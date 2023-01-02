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
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using LittleBlocks.Exports.Csv;
using LittleBlocks.Exports.Extensions;
using LittleBlocks.Exports.UnitTests.Setup;
using LittleBlocks.Testing;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace LittleBlocks.Exports.UnitTests.Csv
{
    public class CsvFileExporterTests
    {
        [Theory]
        [AutoSubstituteAndData]
        public async Task Should_ExportAsync_Succeed_WhenTheInputAndTheConfigurationIsCorrect(
            SampleEntity[] entities,
            ExporterOptions exporterOptions,
            [Frozen] ICsvExportConfigurationBuilder configurationBuilder,
            CsvFileExporter sut)
        {
            // ARRANGE
            configurationBuilder.Build<SampleEntity>(exporterOptions)
                .Returns(new CsvExportConfiguration {FileName = "FilePath"});

            // ACT
            var actual = await sut.ExportAsync(entities, exporterOptions);

            // ASSERT
            actual.Should().NotBeNull();
            actual.HasError.Should().BeFalse();
            actual.RecordCount.Should().Be(entities.Length);
            actual.TargetFile.Should().Be("FilePath");
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
            configurationBuilder.Build<SampleEntity>(exporterOptions).Returns((CsvExportConfiguration) null);

            // ACT
            var actual = await sut.ExportAsync(entities, exporterOptions);

            // ASSERT
            actual.Should().NotBeNull();
            actual.HasError.Should().BeTrue();
            actual.Error.Should()
                .Be(
                    $"Error in creating export configuration '{typeof(SampleEntity)}'. Make sure the ClassMap has been registered");
        }

        [Theory]
        [AutoSubstituteAndData]
        public async Task Should_ExportAsync_FailWithEmptyEntityList_WhenTheInputAndTheConfigurationIsCorrect(
            ExporterOptions exporterOptions,
            [Frozen] ICsvExportConfigurationBuilder configurationBuilder,
            CsvFileExporter sut)
        {
            // ARRANGE
            configurationBuilder.Build<SampleEntity>(exporterOptions)
                .Returns(new CsvExportConfiguration {FileName = "FilePath"});

            // ACT
            var actual = await sut.ExportAsync(new SampleEntity[] { }, exporterOptions);

            // ASSERT
            actual.Should().NotBeNull();
            actual.HasError.Should().BeTrue();
            actual.Error.Should().Be($"Export for {typeof(SampleEntity)} has cancelled due to an empty list of items");
        }

        [Theory]
        [AutoSubstituteAndData]
        public async Task
            Should_ExportAsync_FailWithError_WhenTheInputAndTheConfigurationIsCorrectButOperationThrowException(
                SampleEntity[] entities,
                ExporterOptions exporterOptions,
                [Frozen] ICsvFileWriter fileWriter,
                [Frozen] ICsvExportConfigurationBuilder configurationBuilder,
                CsvFileExporter sut)
        {
            // ARRANGE
            configurationBuilder.Build<SampleEntity>(exporterOptions)
                .Returns(new CsvExportConfiguration {FileName = "FilePath"});
            fileWriter.WriteFileAsync(entities, Arg.Any<CsvExportConfiguration>())
                .Throws(new Exception("Internal Error"));

            // ACT
            var actual = await sut.ExportAsync(entities, exporterOptions);

            // ASSERT
            actual.Should().NotBeNull();
            actual.HasError.Should().BeTrue();
            actual.Error.Should()
                .Be(
                    $"Error in exporting {typeof(SampleEntity)} to target location {exporterOptions.Targets.ToJson()}. Reason: Internal Error");
        }
    }
}