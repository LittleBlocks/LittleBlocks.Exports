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
using Autofac;
using Easify.Exports.Csv;
using Easify.Exports.Storage;
using Easify.Exports.Storage.Fluent.S3;
using Easify.Exports.Storage.Fluent.S3.Fluent;
using Microsoft.Extensions.Configuration;
using Storage.Net;

namespace Easify.Exports.Autofac
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder RegisterCsv(this ContainerBuilder builder,
            Action<ICsvContextMapRegistry> configure)
        {
            builder.RegisterType<CsvFileExporter>().AsImplementedInterfaces();
            builder.RegisterType<DateBasedExportFileNameBuilder>().AsImplementedInterfaces();
            builder.RegisterType<CsvFileWriter>().AsImplementedInterfaces();
            builder.RegisterType<CsvExportConfigurationBuilder>().AsImplementedInterfaces();
            builder.RegisterType<CsvStorageTargetResolver>().IfNotRegistered(typeof(CsvStorageTargetResolver))
                .AsImplementedInterfaces();
            builder.Register(sp =>
            {
                var registry = new CsvContextMapRegistry();

                configure?.Invoke(registry);

                return registry;
            }).AsSelf().AsImplementedInterfaces().SingleInstance();

            return builder;
        }

        public static ContainerBuilder RegisterS3BucketStorageWithSamlSupport(this ContainerBuilder builder,
            IConfiguration configuration, Action<IHaveProfile> configure)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            var options = new BucketOptions();
            configuration.GetSection(nameof(BucketOptions)).Bind(options);

            configure?.Invoke(options);

            builder.RegisterType<CsvStorageTargetResolver>().IfNotRegistered(typeof(CsvStorageTargetResolver))
                .AsImplementedInterfaces();
            builder.Register(sp =>
            {
                var credentials = options.CreateOrRefreshSamlCredentials();
                var blobStorage = StorageFactory.Blobs.AwsS3(credentials, options.Region, options.BucketName);
                return new GenericCsvStorageTarget(StorageTargetType.S3Bucket, blobStorage);
            }).AsImplementedInterfaces().SingleInstance();

            return builder;
        }

        public static ContainerBuilder RegisterS3BucketStorage(this ContainerBuilder builder,
            IConfiguration configuration, Action<IHaveProfile> configure)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            var options = new BucketOptions();
            configuration.GetSection(nameof(BucketOptions)).Bind(options);

            configure?.Invoke(options);

            builder.RegisterType<CsvStorageTargetResolver>().IfNotRegistered(typeof(CsvStorageTargetResolver))
                .AsImplementedInterfaces();
            builder.Register(sp =>
            {
                var blobStorage = StorageFactory.Blobs.AwsS3(options.Profile, options.Region, options.BucketName);
                return new GenericCsvStorageTarget(StorageTargetType.S3Bucket, blobStorage);
            }).AsImplementedInterfaces().SingleInstance();

            return builder;
        }

        public static ContainerBuilder RegisterInMemoryStorage(this ContainerBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.RegisterType<CsvStorageTargetResolver>().IfNotRegistered(typeof(CsvStorageTargetResolver))
                .AsImplementedInterfaces();
            builder.Register(sp =>
            {
                var blobStorage = StorageFactory.Blobs.InMemory();
                return new GenericCsvStorageTarget(StorageTargetType.InMemory, blobStorage);
            }).AsImplementedInterfaces().SingleInstance();

            return builder;
        }

        public static ContainerBuilder RegisterLocalDiskStorage(this ContainerBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.RegisterType<CsvStorageTargetResolver>().IfNotRegistered(typeof(CsvStorageTargetResolver))
                .AsImplementedInterfaces();
            builder.RegisterType<LocalDiskCsvStorageTarget>().AsImplementedInterfaces();

            return builder;
        }
    }
}