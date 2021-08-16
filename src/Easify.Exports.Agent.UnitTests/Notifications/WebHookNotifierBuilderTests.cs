using System;
using Easify.Testing;
using FluentAssertions;
using Easify.Exports.Agent.Notifications;
using Easify.Exports.Common;
using Xunit;

namespace Easify.Exports.Agent.UnitTests.Notifications
{
    public class WebHookNotifierBuilderTests
    {
        [Theory]
        [AutoSubstituteAndData]
        public void Should_NotificationFor_ReturnsValidNotifyResult(
            WebHookNotifierBuilder sut)
        {
            // ARRANGE
            var result = new SuccessNotification(Guid.NewGuid(), Guid.NewGuid(), 10, 14);

            // ACT
            var actual = sut.NotificationFor("url", result);

            // ASSERT
            actual.Should().NotBeNull();
        }
    }
}