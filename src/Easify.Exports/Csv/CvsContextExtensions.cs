using System;
using System.Linq;
using CsvHelper;
using CsvHelper.TypeConversion;

namespace Easify.Exports.Csv
{
    public static class CvsContextExtensions
    {
        public static void SetupFromCsvConfiguration(this CsvContext context, CsvExportConfiguration configuration)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            configuration.ClassMaps.ToList().ForEach(cm => context.RegisterClassMap(cm));

            var typeConverterOptions = new TypeConverterOptions
                {Formats = new[] {configuration.DateTimeFormat ?? ExporterDefaults.DefaultDateTimeFormat}};
            context.TypeConverterOptionsCache.AddOptions<DateTime>(typeConverterOptions);
            context.TypeConverterOptionsCache.AddOptions<DateTime?>(typeConverterOptions);
        }
    }
}