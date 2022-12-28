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
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using LittleBlocks.Exports.Agent.Notifications;
using LittleBlocks.Exports.Client.Exceptions;
using LittleBlocks.Exports.Common;
using LittleBlocks.Exports.Csv;
using LittleBlocks.Exports.Storage;
using LittleBlocks.Testing;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace LittleBlocks.Exports.Agent.UnitTests
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
            public SampleExporter(IReportNotifierBuilder reportNotifierBuilder, ILogger<ExporterBase> logger) : base(
                reportNotifierBuilder, logger)
            {
            }

            protected override Task<ExportResult> InternalRunAsync(ExportExecutionContext context,
                StorageTarget[] storageTargets)
            {
                throw new NotImplementedException();
            }
        }
    }
}