﻿using System.Threading.Tasks;

 namespace Easify.Exports.Agent.Notifications
{
    public interface IReportNotifier
    {
       Task  RunAsync();
    }
}