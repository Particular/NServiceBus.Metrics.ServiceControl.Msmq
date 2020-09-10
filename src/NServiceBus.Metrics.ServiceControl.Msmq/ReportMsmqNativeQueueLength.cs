using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Logging;
using NServiceBus.Transport;

class ReportMsmqNativeQueueLength : Feature
{
    public ReportMsmqNativeQueueLength()
    {
        EnableByDefault();
        DependsOn("NServiceBus.Metrics.ServiceControl.ReportingFeature");
        Prerequisite(ctx => ctx.Settings.Get<TransportDefinition>() is MsmqTransport, "MSMQ Transport not configured");
    }

    protected override void Setup(FeatureConfigurationContext context)
    {
        context.Services.AddSingleton<MsmqNativeQueueLengthReporter>();
        context.Services.AddSingleton<PeriodicallyReportQueueLength>();

        context.RegisterStartupTask(b => new PeriodicallyReportQueueLength(b.GetRequiredService<MsmqNativeQueueLengthReporter>()));
    }

    class PeriodicallyReportQueueLength : FeatureStartupTask
    {
        TimeSpan delayBetweenReports = TimeSpan.FromSeconds(1);
        CancellationTokenSource cancellationTokenSource;
        Task task;
        MsmqNativeQueueLengthReporter reporter;

        public PeriodicallyReportQueueLength(MsmqNativeQueueLengthReporter reporter)
        {
            this.reporter = reporter;
        }

        protected override Task OnStart(IMessageSession messageSession)
        {
            cancellationTokenSource = new CancellationTokenSource();
            task = Task.Run(async () =>
            {
                reporter.Warmup();
                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(delayBetweenReports, cancellationTokenSource.Token).ConfigureAwait(false);
                        reporter.ReportNativeQueueLength();
                    }
                    catch (TaskCanceledException)
                    {
                        // Ignore cancellation. It means we are shutting down
                    }
                    catch (Exception ex)
                    {
                        Log.Warn("Error reporting MSMQ native queue length", ex);
                    }
                }
            });

            return Task.FromResult(0);
        }

        protected override Task OnStop(IMessageSession messageSession)
        {
            cancellationTokenSource.Cancel();

            return task;
        }

        static ILog Log = LogManager.GetLogger<PeriodicallyReportQueueLength>();
    }
}
