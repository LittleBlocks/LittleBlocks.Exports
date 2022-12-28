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

using Autofac;
using Autofac.Extensions.DependencyInjection;
using LittleBlocks.Exports.Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace LittleBlocks.Exports.IntegrationTests.Setup
{
    public class AutofacResolver : IResolver
    {
        private readonly IContainer _container;

        public AutofacResolver()
        {
            var services = new ServiceCollection();
            services.AddLogging();

            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.RegisterInMemoryStorage();
            builder.RegisterCsv(c => { c.Register<SampleEntityMap, SampleEntity>(); });

            _container = builder.Build();
        }

        public T Resolve<T>()
        {
            return _container.Resolve<T>();
        }
    }
}