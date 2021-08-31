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

using Easify.Exports.Storage;
using Easify.Testing;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Easify.Exports.UnitTests
{
    public class ServiceCollectionExtensionsTests : IClassFixture<FixtureBase>
    {
        private readonly FixtureBase _fixture;

        public ServiceCollectionExtensionsTests(FixtureBase fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void Should_AddLocalDiskStorage_RegisterBlobStorageCorrectly()
        {
            // ARRANGE
            var configuration = _fixture.Fake<IConfiguration>();
            configuration.GetSection(Arg.Any<string>()).Returns(_fixture.Fake<IConfigurationSection>());

            var services = new ServiceCollection();
            services.AddLocalDiskStorage();

            var provider = services.BuildServiceProvider();

            // ACT
            var sut = provider.GetRequiredService<ICsvStorageTargetResolver>();

            // ASSERT
            sut.Resolve(StorageTargetType.LocalDisk).Should().NotBeNull();
            sut.Resolve(StorageTargetType.InMemory).Should().BeNull();
            sut.Resolve(StorageTargetType.S3Bucket).Should().BeNull();
        }

        [Fact]
        public void Should_AddInMemoryStorage_RegisterBlobStorageCorrectly()
        {
            // ARRANGE
            var configuration = _fixture.Fake<IConfiguration>();
            configuration.GetSection(Arg.Any<string>()).Returns(_fixture.Fake<IConfigurationSection>());

            var services = new ServiceCollection();
            services.AddInMemoryStorage();

            var provider = services.BuildServiceProvider();

            // ACT
            var sut = provider.GetRequiredService<ICsvStorageTargetResolver>();

            // ASSERT
            sut.Resolve(StorageTargetType.InMemory).Should().NotBeNull();
            sut.Resolve(StorageTargetType.LocalDisk).Should().BeNull();
            sut.Resolve(StorageTargetType.S3Bucket).Should().BeNull();
        }
    }
}