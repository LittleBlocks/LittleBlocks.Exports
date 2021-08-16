namespace Easify.Exports.IntegrationTests.Setup
{
    public interface IResolver
    {
        T Resolve<T>();
    }
}