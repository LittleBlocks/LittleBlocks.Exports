using System;
using System.Collections.Concurrent;
using CsvHelper.Configuration;

namespace Easify.Exports.Csv
{
    public class CsvContextMapRegistry : ICsvContextMapRegistry, ICsvContextMapResolver
    {
        private readonly ConcurrentDictionary<Type, Type> _registry = new ConcurrentDictionary<Type, Type>();
        
        public void Register<TClassMap, T>() 
            where TClassMap : ClassMap<T>
            where T : class
        {
            _registry.AddOrUpdate(typeof(T), typeof(TClassMap), (m, map) => typeof(TClassMap));
        }

        public Type Resolve<T>()
        {
            return _registry.TryGetValue(typeof(T), out var map) ? map : null;
        }
    }
}