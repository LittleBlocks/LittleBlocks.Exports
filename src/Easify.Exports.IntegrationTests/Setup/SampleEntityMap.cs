using CsvHelper.Configuration;

namespace Easify.Exports.IntegrationTests.Setup
{
    public sealed class SampleEntityMap : ClassMap<SampleEntity>
    {
        public SampleEntityMap()
        {
            Map(m => m.Id);
            Map(m => m.Name);
        }
    }
}