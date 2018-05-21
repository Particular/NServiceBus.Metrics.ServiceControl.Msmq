using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using NServiceBus.Metrics.ServiceControl;

class MsmqNativeQueueLengthReporter
{
    IReportNativeQueueLength nativeQueueLengthReporter;
    List<Tuple<string, MessageQueue>> monitoredQueues = new List<Tuple<string, MessageQueue>>();

    public MsmqNativeQueueLengthReporter(IReportNativeQueueLength nativeQueueLengthReporter)
    {
        this.nativeQueueLengthReporter = nativeQueueLengthReporter;
    }

    public void Warmup()
    {
        foreach (var monitoredQueue in nativeQueueLengthReporter.MonitoredQueues)
        {
            var queueName = monitoredQueue.Split('@').FirstOrDefault();

            var messageQueue = new MessageQueue($@".\private$\{queueName}", QueueAccessMode.Peek);

            monitoredQueues.Add(Tuple.Create(monitoredQueue, messageQueue));
        }
    }

    public void ReportNativeQueueLength()
    {
        foreach (var monitoredQueue in monitoredQueues)
        {
            var count = monitoredQueue.Item2.GetCount();
            nativeQueueLengthReporter.ReportQueueLength(monitoredQueue.Item1, count);
        }
    }
}
