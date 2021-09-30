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
using Easify.Exports.Csv;
using Easify.Exports.Storage;
using FluentAssertions;
using Xunit;

namespace Easify.Exports.UnitTests.Csv
{
    public class CsvExportPathBuilderTests
    {
        [Theory]
        [InlineData("", "2019-01-31", "20190131000000.csv")]
        [InlineData("", "0001-01-01", "00010101000000.csv")]
        [InlineData("", "2019-01-31 23:20", "20190131232000.csv")]
        [InlineData("", "2020-01-31 00:00", "20200131000000.csv")]
        [InlineData("Sample", "2019-01-31", "Sample20190131000000.csv")]
        [InlineData("Sample", "0001-01-01", "Sample00010101000000.csv")]
        [InlineData("Sample", "2019-01-31 23:20", "Sample20190131232000.csv")]
        [InlineData("Sample", "2020-01-31 00:00", "Sample20200131000000.csv")]
        public void Should_Build_CreateTheExpectedFilePatten(string prefix, string asOf, string expected)
        {
            // ARRANGE
            var date = DateTime.Parse(asOf);
            var sut = new DateBasedExportFileNameBuilder();

            // ACT
            var actual = sut.Build(new ExporterOptions(date, new StorageTarget[] { }, prefix));

            // ASSERT
            actual.Should().Be(expected);
        }
    }
}