namespace Easify.Exports.Csv
{
    public interface IExportFileNameBuilder
    {
        string Build(ExporterOptions options);
    }
}