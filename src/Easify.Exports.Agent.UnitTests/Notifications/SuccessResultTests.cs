using System;
using FluentAssertions;
using Easify.Exports.Common;
using Xunit;

namespace Easify.Exports.Agent.UnitTests.Notifications
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