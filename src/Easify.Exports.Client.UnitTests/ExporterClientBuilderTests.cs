// This software is part of the Easify.Exports Library
// Copyright (C) 2021 Intermediate Capital Group
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
using AutoFixture.Xunit2;
using Easify.Exports.Client.Exceptions;
using Easify.RestEase;
using Easify.Testing;
using FluentAssertions;
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
            action.Should().Throw<DuplicateExporterException>()
                .WithMessage("Duplicate exporter with name name exists in the cache");
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
            action.Should().Throw<ExporterNotFoundException>().WithMessage(
                "No exporter found with name: name. The name should be added using AddClient in configuration");
        }
    }
}