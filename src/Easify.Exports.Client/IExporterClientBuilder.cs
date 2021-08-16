using Easify.Exports.Common;

namespace Easify.Exports.Client
{
    public interface IExporterClientBuilder
    {
        IExporterClient Build(string name);
    }
}