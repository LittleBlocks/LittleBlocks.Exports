using System;
using FluentAssertions;
using Easify.Exports.Common;
using Xunit;

namespace Easify.Exports.Agent.UnitTests.Notifications
{
    public class FailResultTests
    {
        [Fact]
        public void Should_NotifyAsync_ReturnsValidNotifyResult()
        {
            // ARRANGE
            var exportId = Guid.NewGuid();
            var exportExecutionId = Guid.NewGuid();            
            
            // ACT
            var actual = new FailNotification(exportId, exportExecutionId, "Error in the results");

            // ASSERT
            actual.ExportId.Should().Be(exportId);
            actual.ExportExecutionId.Should().Be(exportExecutionId);
            actual.Error.Should().Be("Error in the results");
        }        
    }
}