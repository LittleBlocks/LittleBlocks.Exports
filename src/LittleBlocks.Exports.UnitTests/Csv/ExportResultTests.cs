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

using LittleBlocks.Exports.Csv;
using FluentAssertions;
using Xunit;

namespace LittleBlocks.Exports.UnitTests.Csv
{
    public class ExportResultTests
    {
        [Fact]
        public void Should_Fail_CreateRightExportResult()
        {
            // ARRANGE
            // ACT
            var sut = ExportResult.Fail("Error Reason");

            // ASSERT
            sut.HasError.Should().BeTrue();
            sut.Error.Should().Be("Error Reason");
            sut.RecordCount.Should().Be(0);
        }

        [Fact]
        public void Should_Success_CreateRightExportResult()
        {
            // ARRANGE
            // ACT
            var sut = ExportResult.Success("FilePath", 10);

            // ASSERT
            sut.HasError.Should().BeFalse();
            sut.Error.Should().BeNull();
            sut.RecordCount.Should().Be(10);
            sut.TargetFile.Should().Be("FilePath");
        }
    }
}