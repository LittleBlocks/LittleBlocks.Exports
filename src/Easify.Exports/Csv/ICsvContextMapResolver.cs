using System;

namespace Easify.Exports.Csv
{
    public interface ICsvContextMapResolver
    {
        Type Resolve<T>();
    }
}