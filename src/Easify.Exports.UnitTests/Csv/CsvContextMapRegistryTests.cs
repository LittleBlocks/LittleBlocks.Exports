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

using Easify.Exports.Csv;
using Easify.Exports.UnitTests.Setup;
using FluentAssertions;
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