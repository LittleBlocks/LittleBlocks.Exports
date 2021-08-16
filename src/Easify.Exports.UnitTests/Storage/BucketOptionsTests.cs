using FluentAssertions;
using Easify.Exports.Storage;
using Easify.Exports.Storage.Fluent.S3;
using Xunit;

namespace Easify.Exports.UnitTests.Storage
{
    public class BucketOptionsTests
    {
        [Fact]
        public void Should_FluentConfiguration_ConfigureTheOptionsCorrectly()
        {
            // ARRANGE
            var sut = new BucketOptions();;

            // ACT
            sut.WithProfile("MO").InRegion("Region").ForBucket("BucketName");

            // ASSERT
            sut.Profile.Should().Be("MO");
            sut.Region.Should().Be("Region");
            sut.BucketName.Should().Be("BucketName");
        }
    }
}