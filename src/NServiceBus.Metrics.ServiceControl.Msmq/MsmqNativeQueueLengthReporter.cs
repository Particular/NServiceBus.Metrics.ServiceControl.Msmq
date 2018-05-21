using System.Messaging;
using NServiceBus;
using NServiceBus.Metrics.ServiceControl;

class MsmqNativeQueueLengthReporter
{
    IReportNativeQueueLength nativeQueueLengthReporter;
    readonly Configure configure;
    string localAddress;
    MessageQueue messageQueue;

    public MsmqNativeQueueLengthReporter(IReportNativeQueueLength nativeQueueLengthReporter, Configure configure)
    {
        this.nativeQueueLengthReporter = nativeQueueLengthReporter;
        this.configure = configure;
    }

    public void Warmup()
    {
        localAddress = configure.LocalAddress.ToString();
        messageQueue = new MessageQueue($@".\private$\{configure.LocalAddress.Queue}", QueueAccessMode.Peek);
    }

    public void ReportNativeQueueLength()
    {
        nativeQueueLengthReporter.ReportQueueLength(localAddress, messageQueue.GetCount());
    }
}
