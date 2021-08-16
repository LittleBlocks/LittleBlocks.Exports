using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Easify.Testing;
using FluentAssertions;
using Easify.Exports.Agent.Notifications;
using Easify.Exports.Client.Exceptions;
using Easify.Exports.Common;
using Easify.Exports.Csv;
using Easify.Exports.Storage;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Easify.Exports.Agent.UnitTests
{
    public class ExporterRegistryTests
    {
        [Theory]
        [AutoSubstituteAndData]
        public void Should_Register_RegisterTheNewExporter_WhenThereIsNothingRegisteredYet(
            ExportMetadata exportMetadata,
            [Frozen] IServiceProvider serviceProvider,
            SampleExporter sampleExporter,
            ExporterRegistry sut)
        {
            // ARRANGE
            serviceProvider.GetService(Arg.Any<Type>()).Returns(sampleExporter);
            
            // ACT
            sut.Register<SampleExporter>(exportMetadata);

            // ASSERT
            sut.Build(exportMetadata.ExportId).Should().BeOfType<SampleExporter>();
        }        
        
        [Theory]
        [AutoSubstituteAndData]
        public void Should_Build_ReturnTheObjectMadeFromContainer_WhenTheMappingIsAvailable(
            ExportMetadata exportMetadata,
            [Frozen] IServiceProvider serviceProvider,
            SampleExporter sampleExporter,
            ExporterRegistry sut)
        {
            // ARRANGE
            serviceProvider.GetService(Arg.Any<Type>()).Returns(sampleExporter);
            sut.Register<SampleExporter>(exportMetadata);
            
            // ACT
            var actual = sut.Build(exportMetadata.ExportId);

            // ASSERT
            actual.Should().BeOfType<SampleExporter>();
            serviceProvider.Received().GetService(typeof(SampleExporter));
        } 
        
        [Theory]
        [AutoSubstituteAndData]
        public void Should_Build_ThrowError_WhenTheMappingIsNotAvailable(
            ExportMetadata exportMetadata,
            [Frozen] IServiceProvider serviceProvider,
            SampleExporter sampleExporter,
            ExporterRegistry sut)
        {
            // ARRANGE
            serviceProvider.GetService(Arg.Any<Type>()).Returns(sampleExporter);
            
            // ACT
            Action action = () => sut.Build(exportMetadata.ExportId);

            // ASSERT
            action.Should().Throw<ExporterNotFoundException>();
        }

        [Theory]
        [AutoSubstituteAndData]
        public void Should_GetRegistrations_ReturnMetaData_WhenTheMappingIsAvailable(
            ExportMetadata exportMetadata,
            [Frozen] IServiceProvider serviceProvider,
            SampleExporter sampleExporter,
            ExporterRegistry sut)
        {
            // ARRANGE
            serviceProvider.GetService(Arg.Any<Type>()).Returns(sampleExporter);
            sut.Register<SampleExporter>(exportMetadata);

            // ACT
            var registrations = sut.GetRegistrations().ToArray();

            // ASSERT
            registrations.Should().HaveCount(1);
            registrations[0].Should().Be(exportMetadata);
        }

        [Theory]
        [AutoSubstituteAndData]
        public void Should_GetRegistrations_ReturnNoMetaData_WhenTheMappingIsNotAvailable(
            [Frozen] IServiceProvider serviceProvider,
            SampleExporter sampleExporter,
            ExporterRegistry sut)
        {
            // ARRANGE
            serviceProvider.GetService(Arg.Any<Type>()).Returns(sampleExporter);

            // ACT
            var registrations = sut.GetRegistrations().ToArray();

            // ASSERT
            registrations.Should().BeEmpty();
        }

        public class SampleExporter : ExporterBase
        {
            
            protected override Task<ExportResult> InternalRunAsync(ExportExecutionContext context,
                StorageTarget[] storageTargets)
            {
                throw new System.NotImplementedException();
            }

            public SampleExporter(IReportNotifierBuilder reportNotifierBuilder, ILogger<ExporterBase> logger) : base(reportNotifierBuilder, logger)
            {
            }
        }
    }
}