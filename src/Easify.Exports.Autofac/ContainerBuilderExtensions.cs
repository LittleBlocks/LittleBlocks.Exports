using System;
using Autofac;
using Easify.Exports.Csv;
using Easify.Exports.Storage;
using Easify.Exports.Storage.Fluent.S3;
using Easify.Exports.Storage.Fluent.S3.Fluent;
using Microsoft.Extensions.Configuration;
using Storage.Net;
using Storage.Net.Amazon.Aws;

namespace Easify.Exports.Autofac
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder RegisterCsv(this ContainerBuilder builder, Action<ICsvContextMapRegistry> configure)
        {
            builder.RegisterType<CsvFileExporter>().AsImplementedInterfaces();
            builder.RegisterType<DateBasedExportFileNameBuilder>().AsImplementedInterfaces();
            builder.RegisterType<CsvFileWriter>().AsImplementedInterfaces();
            builder.RegisterType<CsvExportConfigurationBuilder>().AsImplementedInterfaces();
            builder.RegisterType<CsvStorageTargetResolver>().IfNotRegistered(typeof(CsvStorageTargetResolver)).AsImplementedInterfaces();
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

            builder.RegisterType<CsvStorageTargetResolver>().IfNotRegistered(typeof(CsvStorageTargetResolver)).AsImplementedInterfaces();
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

            builder.RegisterType<CsvStorageTargetResolver>().IfNotRegistered(typeof(CsvStorageTargetResolver)).AsImplementedInterfaces();
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

            builder.RegisterType<CsvStorageTargetResolver>().IfNotRegistered(typeof(CsvStorageTargetResolver)).AsImplementedInterfaces();
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

            builder.RegisterType<CsvStorageTargetResolver>().IfNotRegistered(typeof(CsvStorageTargetResolver)).AsImplementedInterfaces();
            builder.RegisterType<LocalDiskCsvStorageTarget>().AsImplementedInterfaces();

            return builder;
        }
    }
}