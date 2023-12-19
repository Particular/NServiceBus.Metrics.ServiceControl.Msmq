# NServiceBus.Metrics.ServiceControl.Msmq

NServiceBus.Metrics.ServiceControl.Msmq provides native queue length reporting to ServiceControl Monitoring for endpoints running on the MSMQ transport. It does it by monitoring the  endpoints' queue length and then passing that data to NServiceBus.Metrics.ServiceControl which in turn sends it to an instance of the ServiceControl.Monitoring service.

See the [Setup Queue Length Metrics Reporting for the MSMQ Transport documentation](https://docs.particular.net/monitoring/metrics/msmq-queue-length) for more information.

## Prerequisites

NServiceBus.Metrics.ServiceControl.Msmq requires the .NET Framework and can only be used with NServiceBus 8 and below.
