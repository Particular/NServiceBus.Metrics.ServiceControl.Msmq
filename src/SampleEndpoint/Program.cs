namespace SampleEndpoint
{
    using System;
    using System.Threading.Tasks;
    using NServiceBus;

    class Program
    {
        static async Task Main(string[] args)
        {
            var endpointConfig = new EndpointConfiguration("SomeName");
            endpointConfig.UseTransport<MsmqTransport>();
            endpointConfig.EnableMetrics()
                .SendMetricDataToServiceControl("Particular.Monitoring", TimeSpan.FromMilliseconds(500));
            endpointConfig.UsePersistence<InMemoryPersistence>();
            endpointConfig.AuditProcessedMessagesTo("audit");
            endpointConfig.SendFailedMessagesTo("error");
            endpointConfig.LimitMessageProcessingConcurrencyTo(1);
            endpointConfig.EnableInstallers();
            endpointConfig.OverrideLocalAddress("SomeOtherName");

            var endpoint = await Endpoint.Start(endpointConfig).ConfigureAwait(false);

            Console.WriteLine("Endpoint Started");

            while (Console.ReadKey(true).Key != ConsoleKey.Escape)
            {
                await endpoint.SendLocal(new SomeMessage()).ConfigureAwait(false);
            }

            await endpoint.Stop().ConfigureAwait(false);

            Console.WriteLine("Bus Stopped");
        }
    }

    class SomeMessage : ICommand
    {

    }

    class SomeMessageHandler : IHandleMessages<SomeMessage>
    {
        public Task Handle(SomeMessage message, IMessageHandlerContext context)
        {
            return Task.Delay(500);
        }
    }
}
