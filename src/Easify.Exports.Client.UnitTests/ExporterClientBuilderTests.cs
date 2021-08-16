using System;
using AutoFixture.Xunit2;
using Easify.RestEase;
using Easify.Testing;
using FluentAssertions;
using Easify.Exports.Client.Exceptions;
using NSubstitute;
using Xunit;

namespace Easify.Exports.Client.UnitTests
{
    public class ExporterClientBuilderTests
    {
        [Theory]
        [AutoSubstituteAndData]
        public void Should_AddClient_RunSuccessfully_WhenAddingNewClientWithValidUrl(
            [Frozen] IRestClientBuilder builder, 
            IExporterClient exporterClient,
            ExporterClientBuilder sut)
        {
            // ARRANGE
            builder.Build<IExporterClient>(Arg.Any<string>()).Returns(exporterClient);

            // ACT
            var actual = sut.AddClient("name", "http://localhost");

            // ASSERT
            actual.Should().Be(sut);
        }        
        
        [Theory]
        [AutoSubstituteAndData]
        public void Should_AddClient_ThrowDuplicateError_WhenAddingClientTwiceWithValidUrl(
            [Frozen] IRestClientBuilder builder, 
            IExporterClient exporterClient,
            ExporterClientBuilder sut)
        {
            // ARRANGE
            builder.Build<IExporterClient>(Arg.Any<string>()).Returns(exporterClient);

            // ACT
            Action action = () => sut.AddClient("name", "http://localhost").AddClient("name", "http://localhost");

            // ASSERT
            action.Should().Throw<DuplicateExporterException>().WithMessage($"Duplicate exporter with name name exists in the cache");
        }     
        
        [Theory]
        [AutoSubstituteAndData]
        public void Should_AddClient_ThrowUrlFormatError_WhenAddingNewClientWithInvalidUrl(
            [Frozen] IRestClientBuilder builder, 
            IExporterClient exporterClient,
            ExporterClientBuilder sut)
        {
            // ARRANGE
            builder.Build<IExporterClient>(Arg.Any<string>()).Returns(exporterClient);

            // ACT
            Action action = () => sut.AddClient("name", "localhost");

            // ASSERT
            action.Should().Throw<InvalidUrlFormatException>().WithMessage("The url localhost is not wellFormed");
        }        
        
        [Theory]
        [AutoSubstituteAndData]
        public void Should_Build_CreateNewExporter_WhenTheExporterNameHasBeenAddedBefore(
            [Frozen] IRestClientBuilder builder, 
            IExporterClient exporterClient,
            ExporterClientBuilder sut)
        {
            // ARRANGE
            builder.Build<IExporterClient>(Arg.Any<string>()).Returns(exporterClient);
            sut.AddClient("name", "http://localhost");

            // ACT
            var actual = sut.Build("name");

            // ASSERT
            actual.Should().NotBeNull();
        }
        
        [Theory]
        [AutoSubstituteAndData]
        public void Should_Build_ThrowNotFoundError_WhenTheExporterNameHasNotBeenAddedBefore(
            [Frozen] IRestClientBuilder builder, 
            IExporterClient exporterClient,
            ExporterClientBuilder sut)
        {
            // ARRANGE
            builder.Build<IExporterClient>(Arg.Any<string>()).Returns(exporterClient);
            
            // ACT
            Action action = () => sut.Build("name");

            // ASSERT
            action.Should().Throw<ExporterNotFoundException>().WithMessage($"No exporter found with name: name. The name should be added using AddClient in configuration");
        }
    }
}