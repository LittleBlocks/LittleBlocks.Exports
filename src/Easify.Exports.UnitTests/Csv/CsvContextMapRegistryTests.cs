using FluentAssertions;
using Easify.Exports.Csv;
using Easify.Exports.UnitTests.Setup;
using Xunit;

namespace Easify.Exports.UnitTests.Csv
{
    public class CsvContextMapRegistryTests
    {
        [Fact]
        public void Should_Register_RegisterTheNewClassMap_WhenThereIsNothingRegisteredYet()
        {
            // ARRANGE
            var sut = new CsvContextMapRegistry();
            
            // ACT
            sut.Register<SampleEntityMap, SampleEntity>();

            // ASSERT
            sut.Resolve<SampleEntity>().Should().Be<SampleEntityMap>();
        }        
        
        [Fact]
        public void Should_Resolve_ReturnTheRegisteredMap_WhenTheMappingIsAvailable()
        {
            // ARRANGE
            var sut = new CsvContextMapRegistry();
            sut.Register<SampleEntityMap, SampleEntity>();
            
            // ACT
            var actual = sut.Resolve<SampleEntity>();

            // ASSERT
            actual.Should().Be<SampleEntityMap>();
        } 
        
        [Fact]
        public void Should_Resolve_ReturnNull_WhenTheMappingIsUnavailable()
        {
            // ARRANGE
            var sut = new CsvContextMapRegistry();
            
            // ACT
            var actual = sut.Resolve<SampleEntity>();

            // ASSERT
            actual.Should().Be(null);
        }
    }
}