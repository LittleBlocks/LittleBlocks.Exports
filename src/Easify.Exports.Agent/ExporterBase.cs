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
        private readonly IReportNotifierBuilder _reportNotifierBuilder;
        private readonly ILogger<ExporterBase> _logger;

        protected ExporterBase(IReportNotifierBuilder reportNotifierBuilder, ILogger<ExporterBase> logger)
        {
            _reportNotifierBuilder = reportNotifierBuilder ?? throw new ArgumentNullException(nameof(reportNotifierBuilder));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public virtual async Task RunAsync(ExportExecutionContext executionContext, StorageTarget[] storageTargets)
        {
            if (executionContext == null) throw new ArgumentNullException(nameof(executionContext));
            if (storageTargets == null) throw new ArgumentNullException(nameof(storageTargets));

            try
            {
                _logger.LogInformation($"Start of the export process for {GetType().Name}. Context: {executionContext.ToJson()}");

                var stopWatch = Stopwatch.StartNew();
                var result = await InternalRunAsync(executionContext, storageTargets);
                stopWatch.Stop();

                if (result.HasError)
                {
                    _logger.LogError($"Error in the export process for {GetType().Name}. Error: {result.Error}. Context: {executionContext.ToJson()}");
                    await _reportNotifierBuilder.NotificationFor(executionContext.FailWebHook, FailNotification.From(executionContext, result.Error)).RunAsync();
                    return;
                }

                await _reportNotifierBuilder.NotificationFor(executionContext.SuccessWebHook,
                    SuccessNotification.From(executionContext, result.RecordCount, (long)stopWatch.Elapsed.TotalSeconds)).RunAsync();

                _logger.LogInformation($"Completion of the export process for {GetType().Name}. Result: {result.ToJson()}");
            }
            catch (Exception e)
            {
                _logger.LogError($"Error in the export process for {GetType().Name}. Context: {executionContext.ToJson()}", e);
                await _reportNotifierBuilder.NotificationFor(executionContext.FailWebHook, FailNotification.From(executionContext, e.ToString())).RunAsync();
            }
        }

        protected abstract Task<ExportResult> InternalRunAsync(ExportExecutionContext context, StorageTarget[] storageTargets);
    }
}