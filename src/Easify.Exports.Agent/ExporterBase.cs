// This software is part of the Easify.Exports Library
// Copyright (C) 2021 Intermediate Capital Group
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
using System.Diagnostics;
using System.Threading.Tasks;
using Easify.Exports.Agent.Notifications;
using Easify.Exports.Common;
using Easify.Exports.Csv;
using Easify.Exports.Extensions;
using Easify.Exports.Storage;
using Microsoft.Extensions.Logging;

namespace Easify.Exports.Agent
{
    public abstract class ExporterBase : IExporter
    {
        private readonly ILogger<ExporterBase> _logger;
        private readonly IReportNotifierBuilder _reportNotifierBuilder;

        protected ExporterBase(IReportNotifierBuilder reportNotifierBuilder, ILogger<ExporterBase> logger)
        {
            _reportNotifierBuilder =
                reportNotifierBuilder ?? throw new ArgumentNullException(nameof(reportNotifierBuilder));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public virtual async Task RunAsync(ExportExecutionContext executionContext, StorageTarget[] storageTargets)
        {
            if (executionContext == null) throw new ArgumentNullException(nameof(executionContext));
            if (storageTargets == null) throw new ArgumentNullException(nameof(storageTargets));

            try
            {
                _logger.LogInformation(
                    $"Start of the export process for {GetType().Name}. Context: {executionContext.ToJson()}");

                var stopWatch = Stopwatch.StartNew();
                var result = await InternalRunAsync(executionContext, storageTargets);
                stopWatch.Stop();

                if (result.HasError)
                {
                    _logger.LogError(
                        $"Error in the export process for {GetType().Name}. Error: {result.Error}. Context: {executionContext.ToJson()}");
                    await _reportNotifierBuilder.NotificationFor(executionContext.FailWebHook,
                        FailNotification.From(executionContext, result.Error)).RunAsync();
                    return;
                }

                await _reportNotifierBuilder.NotificationFor(executionContext.SuccessWebHook,
                    SuccessNotification.From(executionContext, result.RecordCount,
                        (long) stopWatch.Elapsed.TotalSeconds)).RunAsync();

                _logger.LogInformation(
                    $"Completion of the export process for {GetType().Name}. Result: {result.ToJson()}");
            }
            catch (Exception e)
            {
                _logger.LogError(
                    $"Error in the export process for {GetType().Name}. Context: {executionContext.ToJson()}", e);
                await _reportNotifierBuilder.NotificationFor(executionContext.FailWebHook,
                    FailNotification.From(executionContext, e.ToString())).RunAsync();
            }
        }

        protected abstract Task<ExportResult> InternalRunAsync(ExportExecutionContext context,
            StorageTarget[] storageTargets);
    }
}