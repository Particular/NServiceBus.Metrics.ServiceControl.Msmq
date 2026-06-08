# NServiceBus.Metrics.ServiceControl.Msmq

> [!IMPORTANT]
> The most recent version of this library is covered by the [extended support policy](https://docs.particular.net/nservicebus/upgrades/support-policy#extended-support) until 2031-04-16.
> For users with extended support, they can raise issues using the [Support Portal](https://customers.particular.net/login?redirect=/request-support).

NServiceBus.Metrics.ServiceControl.Msmq provides native queue length reporting to ServiceControl Monitoring for endpoints running on the MSMQ transport. It does it by monitoring the endpoints' queue length and then passing that data to NServiceBus.Metrics.ServiceControl which in turn sends it to an instance of the ServiceControl.Monitoring service.

See the [Setup Queue Length Metrics Reporting for the MSMQ Transport documentation](https://docs.particular.net/monitoring/metrics/msmq-queue-length) for more information.

## Prerequisites

NServiceBus.Metrics.ServiceControl.Msmq requires the .NET Framework and can only be used with NServiceBus 8 and below.
