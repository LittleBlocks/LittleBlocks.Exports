using System;
using FluentAssertions;
using Easify.Exports.Csv;
using Easify.Exports.Storage;
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
        [InlineData("Sample","2019-01-31", "Sample20190131000000.csv")]
        [InlineData("Sample","0001-01-01", "Sample00010101000000.csv")]
        [InlineData("Sample","2019-01-31 23:20", "Sample20190131232000.csv")]
        [InlineData("Sample","2020-01-31 00:00", "Sample20200131000000.csv")]
        public void Should_Build_CreateTheExpectedFilePatten(string prefix, string asOf, string expected)
        {
            // ARRANGE
            var date = DateTime.Parse(asOf);
            var sut = new DateBasedExportFileNameBuilder();

            // ACT
            var actual = sut.Build(new ExporterOptions(date, new StorageTarget[]{}, prefix));

            // ASSERT
            actual.Should().Be(expected);
        }
    }
}