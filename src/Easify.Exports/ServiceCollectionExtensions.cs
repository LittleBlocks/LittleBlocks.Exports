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
using Easify.Exports.Csv;
using Easify.Exports.Storage;
using Easify.Exports.Storage.Fluent.AzureBlobs;
using Easify.Exports.Storage.Fluent.S3;
using Easify.Exports.Storage.Fluent.S3.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Storage.Net;

namespace Easify.Exports
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCsv(this IServiceCollection services,
            Action<ICsvContextMapRegistry> configure)
        {
            services.AddTransient<IFileExporter, CsvFileExporter>();
            services.AddTransient<IExportFileNameBuilder, DateBasedExportFileNameBuilder>();
            services.AddTransient<ICsvFileWriter, CsvFileWriter>();
            services.AddTransient<IFileExporter, CsvFileExporter>();
            services.AddTransient<ICsvStorageTargetResolver, CsvStorageTargetResolver>();

            services.AddTransient<ICsvExportConfigurationBuilder, CsvExportConfigurationBuilder>();
            services.AddSingleton(sp =>
            {
                var registry = new CsvContextMapRegistry();

                configure?.Invoke(registry);

                return registry;
            });
            services.AddSingleton<ICsvContextMapResolver>(sp => sp.GetRequiredService<CsvContextMapRegistry>());
            services.TryAddTransient<ICsvStorageTargetResolver, CsvStorageTargetResolver>();

            return services;
        }

        public static IServiceCollection AddS3BucketStorageWithSamlSupport(this IServiceCollection services,
            IConfiguration configuration, Action<IHaveProfile> configure = null)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var options = new BucketOptions();
            configuration.GetSection(nameof(BucketOptions)).Bind(options);

            configure?.Invoke(options);

            services.TryAddTransient<ICsvStorageTargetResolver, CsvStorageTargetResolver>();
            services.AddScoped<ICsvStorageTarget>(sp =>
            {
                var credentials = options.CreateOrRefreshSamlCredentials();
                var blobStorage = StorageFactory.Blobs.AwsS3(credentials, options.BucketName, options.Region);
                return new GenericCsvStorageTarget(StorageTargetType.S3Bucket, blobStorage);
            });

            return services;
        }

        public static IServiceCollection AddS3BucketStorage(this IServiceCollection services,
            IConfiguration configuration, Action<IHaveProfile> configure = null)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var options = new BucketOptions();
            configuration.GetSection(nameof(BucketOptions)).Bind(options);

            configure?.Invoke(options);

            services.TryAddTransient<ICsvStorageTargetResolver, CsvStorageTargetResolver>();
            services.AddTransient<ICsvStorageTarget>(sp =>
            {
                var blobStorage = StorageFactory.Blobs.AwsS3(options.Profile, options.BucketName, options.Region);
                return new GenericCsvStorageTarget(StorageTargetType.S3Bucket, blobStorage);
            });

            return services;
        }

        public static IServiceCollection AddAccountWithSharedKeyStorage(this IServiceCollection services,
            IConfiguration configuration, Action<IHaveSharedKeyAccount> configure = null)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var options = new BlobSharedKeyOptions();
            configuration.GetSection(nameof(BlobSharedKeyOptions)).Bind(options);

            configure?.Invoke(options);

            services.TryAddTransient<ICsvStorageTargetResolver, CsvStorageTargetResolver>();
            services.AddTransient<ICsvStorageTarget>(sp =>
            {
                var blobStorage = StorageFactory.Blobs.AzureBlobStorageWithSharedKey(options.Name, options.Key);
                return new GenericCsvStorageTarget(StorageTargetType.BlobStorage, blobStorage);
            });

            return services;
        }

        public static IServiceCollection AddAccountWithAzureAdStorage(this IServiceCollection services,
            IConfiguration configuration, Action<IHaveAzureAdAccount> configure = null)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var options = new BlobAzureAdOptions();
            configuration.GetSection(nameof(BlobAzureAdOptions)).Bind(options);

            configure?.Invoke(options);

            services.TryAddTransient<ICsvStorageTargetResolver, CsvStorageTargetResolver>();
            services.AddTransient<ICsvStorageTarget>(sp =>
            {
                var blobStorage = StorageFactory.Blobs.AzureBlobStorageWithAzureAd(options.Name, options.TenantId,
                    options.ApplicationId, options.ApplicationSecret);
                return new GenericCsvStorageTarget(StorageTargetType.BlobStorage, blobStorage);
            });

            return services;
        }

        public static IServiceCollection AddAccountWithEmulatorStorage(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.TryAddTransient<ICsvStorageTargetResolver, CsvStorageTargetResolver>();
            services.AddTransient<ICsvStorageTarget>(sp =>
            {
                var blobStorage = StorageFactory.Blobs.AzureBlobStorageWithLocalEmulator();
                return new GenericCsvStorageTarget(StorageTargetType.BlobStorage, blobStorage);
            });

            return services;
        }

        public static IServiceCollection AddInMemoryStorage(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.TryAddTransient<ICsvStorageTargetResolver, CsvStorageTargetResolver>();
            services.AddScoped<ICsvStorageTarget>(sp =>
            {
                var blobStorage = StorageFactory.Blobs.InMemory();
                return new GenericCsvStorageTarget(StorageTargetType.InMemory, blobStorage);
            });

            return services;
        }

        public static IServiceCollection AddLocalDiskStorage(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.TryAddTransient<ICsvStorageTargetResolver, CsvStorageTargetResolver>();
            services.AddScoped<ICsvStorageTarget, LocalDiskCsvStorageTarget>();

            return services;
        }
    }
}