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

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LittleBlocks.Exports.Storage;
using LittleBlocks.Testing;
using NSubstitute;
using Storage.Net.Blobs;
using Xunit;

namespace LittleBlocks.Exports.UnitTests.Storage
{
    public class CsvStorageTargetTests
    {
        [Theory]
        [AutoSubstituteAndData]
        public async Task Should_Resolve_ReturnCorrectStorageTarget_WhenItIsPresented(IBlobStorage blobStorage)
        {
            // Arrange
            var sut = new GenericCsvStorageTarget(StorageTargetType.InMemory, blobStorage);
            var expected = "Test/Storage/File.csv";

            // Act
            await sut.WriteAsync("Test/Storage/", "File.csv", new byte[] {1, 12, 123});

            // Assert
            await blobStorage.Received(1)
                .WriteAsync(expected, Arg.Any<Stream>(), Arg.Any<bool>(), Arg.Any<CancellationToken>());
        }
    }
}