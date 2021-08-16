using CsvHelper.Configuration;

namespace Easify.Exports.Csv
{
    public interface ICsvContextMapRegistry
    {
        void Register<TClassMap, T>()
            where TClassMap : ClassMap<T>
            where T : class;
    }
}