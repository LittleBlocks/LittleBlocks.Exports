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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using CsvHelper.Configuration;
using LittleBlocks.Exports.Csv;
using LittleBlocks.Exports.Storage;
using LittleBlocks.Exports.UnitTests.Setup;
using LittleBlocks.Testing;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace LittleBlocks.Exports.UnitTests.Csv
{
    public class CsvFileWriterTests
    {
        [Theory]
        [AutoSubstituteAndData]
        public async Task
            Should_CsvFileWriter_CallsTheRightStorageTarget_WhenTheConfigurationIsCorrectAndTargetHasSpecified(
                IEnumerable<SampleEntity> entities,
                ICsvStorageTarget storageTargetBucket,
                ICsvStorageTarget storageTargetFile,
                [Frozen] ICsvStorageTargetResolver storageTargetResolver,
                CsvFileWriter sut)
        {
            // Arrange
            var config = CreateConfiguration();
            storageTargetResolver.Resolve(StorageTargetType.S3Bucket).Returns(storageTargetBucket);
            storageTargetResolver.Resolve(StorageTargetType.LocalDisk).Returns(storageTargetFile);

            // Act
            await sut.WriteFileAsync(entities, config);

            await storageTargetBucket.Received(1).WriteAsync("Target", config.FileName, Arg.Any<byte[]>());
            await storageTargetFile.DidNotReceive().WriteAsync("Target", config.FileName, Arg.Any<byte[]>());
        }

        [Theory]
        [AutoSubstituteAndData]
        public async Task Should_CsvFileWriter_Fail_WhenTheConfigurationIsCorrectButTheTargetSourceIsNotPresented(
            IEnumerable<SampleEntity> entities,
            ICsvStorageTarget storageTargetMemory,
            ICsvStorageTarget storageTargetFile,
            [Frozen] ICsvStorageTargetResolver storageTargetResolver,
            CsvFileWriter sut)
        {
            // Arrange
            var config = CreateConfiguration();
            storageTargetResolver.Resolve(StorageTargetType.LocalDisk).Returns(storageTargetFile);
            storageTargetResolver.Resolve(StorageTargetType.InMemory).Returns(storageTargetMemory);
            storageTargetResolver.Resolve(StorageTargetType.S3Bucket).Returns((ICsvStorageTarget) null);

            // Act
            Func<Task> func = async () => await sut.WriteFileAsync(entities, config);

            await func.Should().ThrowAsync<StorageTargetException>()
                .Where(m => m.Message.StartsWith("Error in writing the file to multiple storage."))
                .Where(m =>
                    m.Exceptions.Count() == 1 &&
                    m.Exceptions.First() is StorageTargetNotFoundException &&
                    m.Exceptions.First().Message.StartsWith("The ExportStorage for S3Bucket is not found"));

            await storageTargetMemory.Received(1).WriteAsync("Target", config.FileName, Arg.Any<byte[]>());
        }

        [Theory]
        [AutoSubstituteAndData]
        public async Task Should_CsvFileWriter_Fail_WhenTheConfigurationIsCorrectButTheTargetSourceIsFailing(
            IEnumerable<SampleEntity> entities,
            ICsvStorageTarget storageTargetMemory,
            ICsvStorageTarget storageTargetFile,
            ICsvStorageTarget storageTargetS3Bucket,
            [Frozen] ICsvStorageTargetResolver storageTargetResolver,
            CsvFileWriter sut)
        {
            // Arrange
            var config = CreateConfiguration();
            storageTargetResolver.Resolve(StorageTargetType.LocalDisk).Returns(storageTargetFile);
            storageTargetResolver.Resolve(StorageTargetType.InMemory).Returns(storageTargetMemory);
            storageTargetResolver.Resolve(StorageTargetType.S3Bucket).Returns(storageTargetS3Bucket);

            storageTargetS3Bucket.WriteAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<byte[]>())
                .Throws(new Exception("Error"));

            // Act
            Func<Task> func = async () => await sut.WriteFileAsync(entities, config);

            await func.Should().ThrowAsync<StorageTargetException>()
                .Where(m => m.Message.StartsWith("Error in writing the file to multiple storage. Error"))
                .Where(m =>
                    m.Exceptions.Count() == 1 &&
                    m.Exceptions.First() != null &&
                    m.Exceptions.First().Message.StartsWith("Error"));


            await storageTargetMemory.Received(1).WriteAsync("Target", config.FileName, Arg.Any<byte[]>());
        }

        private static CsvExportConfiguration CreateConfiguration()
        {
            var config = new CsvExportConfiguration
            {
                FileName = "FilePath.csv",
                Configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = "|",
                    TrimOptions = TrimOptions.InsideQuotes,
                    HasHeaderRecord = true
                },
                Targets = new[]
                {
                    new StorageTarget
                        {TargetLocation = "Target", StorageTargetType = StorageTargetType.InMemory, RetryDelay = 1},
                    new StorageTarget
                        {TargetLocation = "Target", StorageTargetType = StorageTargetType.S3Bucket, RetryDelay = 2}
                },
                ClassMaps = new[] {typeof(SampleEntityMap)}
            };
            return config;
        }

        [Theory]
        [AutoSubstituteAndData]
        public async Task Should_WriteFileAsync_RetiresFourTimes_WhenTheExceptionIsHappening(
            IEnumerable<SampleEntity> entities,
            ICsvStorageTarget storageTargetBucket,
            [Frozen] ICsvStorageTargetResolver storageTargetResolver,
            [Frozen] ILogger<CsvFileWriter> logger,
            CsvFileWriter sut)
        {
            // Arrange
            var config = CreateConfiguration();
            storageTargetResolver.Resolve(StorageTargetType.S3Bucket).Returns(storageTargetBucket);
            storageTargetBucket.WriteAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<byte[]>())
                .Throws(new Exception("Something is wrong."));

            // Act
            Func<Task> func = async () => await sut.WriteFileAsync(entities, config);

            // Assert
            await func.Should().ThrowAsync<StorageTargetException>()
                .Where(m => m.Message.StartsWith("Error in writing the file to multiple storage"));
            logger.Received(3);
            await storageTargetBucket.Received(4).WriteAsync("Target", config.FileName, Arg.Any<byte[]>());
        }
    }
}