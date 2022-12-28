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

using LittleBlocks.Exports.Storage;
using LittleBlocks.Testing;
using FluentAssertions;
using Storage.Net.Blobs;
using Xunit;

namespace LittleBlocks.Exports.UnitTests.Storage
{
    public class CsvStorageTargetResolverTests
    {
        [Theory]
        [AutoSubstituteAndData]
        public void Should_Resolve_ReturnCorrectStorageTarget_WhenItIsPresented(IBlobStorage blobStorage)
        {
            // Arrange
            var sut = new CsvStorageTargetResolver(new[]
                {new GenericCsvStorageTarget(StorageTargetType.InMemory, blobStorage)});

            // Act
            var actual = sut.Resolve(StorageTargetType.InMemory);

            // Assert
            actual.Should().NotBeNull();
        }

        [Theory]
        [AutoSubstituteAndData]
        public void Should_Resolve_ReturnNull_WhenItIsNotPresented(IBlobStorage blobStorage)
        {
            // Arrange
            var sut = new CsvStorageTargetResolver(new[]
                {new GenericCsvStorageTarget(StorageTargetType.InMemory, blobStorage)});

            // Act
            var actual = sut.Resolve(StorageTargetType.LocalDisk);

            // Assert
            actual.Should().BeNull();
        }
    }
}