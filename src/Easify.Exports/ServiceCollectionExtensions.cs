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
using Storage.Net.Amazon.Aws;

namespace Easify.Exports
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCsv(this IServiceCollection services, Action<ICsvContextMapRegistry> configure)
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
                var blobStorage = StorageFactory.Blobs.AzureBlobStorageWithAzureAd(options.Name, options.TenantId, options.ApplicationId, options.ApplicationSecret);
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