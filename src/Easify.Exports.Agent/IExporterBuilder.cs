﻿using System;

 namespace Easify.Exports.Agent
{
    public interface IExporterBuilder
    {
        IExporter Build(Guid exporterId);
    }
}