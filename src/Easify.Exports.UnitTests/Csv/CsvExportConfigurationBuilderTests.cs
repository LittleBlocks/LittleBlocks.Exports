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
using AutoFixture.Xunit2;
using CsvHelper.Configuration;
using Easify.Exports.Csv;
using Easify.Exports.Storage;
using Easify.Exports.UnitTests.Setup;
using Easify.Testing;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Easify.Exports.UnitTests.Csv
{
    public class CsvExportConfigurationBuilderTests
    {
        [Theory]
        [AutoSubstituteAndData]
        public void
            Should_CsvFileWriterContextBuilder_ReturnsCorrectCsvFileWriterContext_WhenItContainsValidExportDataAndClassMap(
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
            actual.FileName.Should().Be("20200101.csv");
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