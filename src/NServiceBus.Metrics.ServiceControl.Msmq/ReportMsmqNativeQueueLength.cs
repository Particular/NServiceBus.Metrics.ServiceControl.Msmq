using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Logging;
using NServiceBus.Transports;

class ReportMsmqNativeQueueLength : Feature
{
    public ReportMsmqNativeQueueLength()
    {
        EnableByDefault();
        DependsOn("ServiceControlMonitoring");
        Prerequisite(ctx => ctx.Settings.Get<TransportDefinition>() is MsmqTransport, "MSMQ Transport not configured");
    }

    protected override void Setup(FeatureConfigurationContext context)
    {
        context.Container.ConfigureComponent<MsmqNativeQueueLengthReporter>(DependencyLifecycle.SingleInstance);
        context.Container.ConfigureComponent<PeriodicallyReportQueueLength>(DependencyLifecycle.SingleInstance);

        RegisterStartupTask<PeriodicallyReportQueueLength>();
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

        protected override void OnStart()
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
        }

        protected override void OnStop()
        {
            cancellationTokenSource.Cancel();
            task.GetAwaiter().GetResult();
        }

        static ILog Log = LogManager.GetLogger<PeriodicallyReportQueueLength>();
    }
}
