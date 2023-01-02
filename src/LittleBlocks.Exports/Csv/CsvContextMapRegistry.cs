// This software is part of the LittleBlocks.Exports Library
// Copyright (C) 2021 LittleBlocks
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System;
using System.Collections.Concurrent;
using CsvHelper.Configuration;

namespace LittleBlocks.Exports.Csv
{
    public class CsvContextMapRegistry : ICsvContextMapRegistry, ICsvContextMapResolver
    {
        private readonly ConcurrentDictionary<Type, Type> _registry = new();

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