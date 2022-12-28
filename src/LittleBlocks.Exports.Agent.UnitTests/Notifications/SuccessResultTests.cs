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
using LittleBlocks.Exports.Common;
using FluentAssertions;
using Xunit;

namespace LittleBlocks.Exports.Agent.UnitTests.Notifications
{
    public class SuccessResultTests
    {
        [Fact]
        public void Should_NotifyAsync_ReturnsValidNotifyResult()
        {
            // ARRANGE
            var exportId = Guid.NewGuid();
            var exportExecutionId = Guid.NewGuid();

            // ACT
            var actual = new SuccessNotification(exportId, exportExecutionId, 10, 14);

            // ASSERT
            actual.ExportId.Should().Be(exportId);
            actual.ExportExecutionId.Should().Be(exportExecutionId);
            actual.NumberOfRecords.Should().Be(10);
            actual.Duration.Should().Be(14);
        }
    }
}