namespace Easify.Exports.Client
{
    public interface IExporterClientRegistry
    {
        IExporterClientRegistry AddClient(string name, string clientUrl);
    }
}